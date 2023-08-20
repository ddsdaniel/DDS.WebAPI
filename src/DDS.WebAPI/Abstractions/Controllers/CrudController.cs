using AutoMapper;
using DDS.Domain.Core.Abstractions.Models.Entities;
using DDS.Domain.Core.Abstractions.Services;
using DDS.WebAPI.Abstractions.ViewModels;
using Flunt.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDS.WebAPI.Abstractions.Controllers
{
    /// <summary>
    /// Controller que provê as implementações genéricas para métodos de CRUD das outras entidades
    /// </summary>
    /// <typeparam name="TViewModelCadastro">View model com os dados necessários para o cadastro</typeparam>
    /// <typeparam name="TViewModelConsulta">View model com os dados que podem ser exibidos no front</typeparam>
    /// <typeparam name="TEntity">Entidade do domínio</typeparam>
    public abstract class CrudController<TViewModelCadastro, TViewModelConsulta, TEntity> : GenericController
        where TViewModelCadastro : IViewModel
        where TEntity : Entity
    {

        private readonly ICrudService<TEntity> _crudService;
        protected readonly IMapper _mapper;


        /// <summary>
        /// Construtor com parâmetros passados a partir da base concreta
        /// </summary>
        /// <param name="crudService">Serviço para validação e manipulação da entidade</param>
        /// /// <param name="mapper">Automapper</param>
        public CrudController(ICrudService<TEntity> crudService,
                              IMapper mapper)
        {
            _crudService = crudService;
            _mapper = mapper;
        }


        /// <summary>
        /// Consulta todos os registros do repositório
        /// </summary>
        /// <returns>Lista de registros (view model)</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IViewModel>), StatusCodes.Status200OK)]
        public virtual ActionResult<IEnumerable<TViewModelCadastro>> ConsultarTodos()
        {
            var lista = _crudService.ConsultarTodos();

            var listaViewModel = _mapper.Map<IEnumerable<TViewModelConsulta>>(lista);

            listaViewModel = Ordenar(listaViewModel);

            return Ok(listaViewModel);
        }

        /// <summary>
        /// Pesquisa sobre os registros do repositório
        /// </summary>
        /// <param name="filtro">Filtro inserido pelo usuário</param>
        /// <returns>Lista de registros correspondentes ao filtro</returns>
        [HttpGet("pesquisa")]
        [ProducesResponseType(typeof(IEnumerable<IViewModel>), StatusCodes.Status200OK)]
        public virtual ActionResult<IEnumerable<TViewModelCadastro>> Pesquisar([FromQuery] string filtro)
        {
            var lista = _crudService.Pesquisar(filtro);
            
            var listaViewModel = _mapper.Map<IEnumerable<TViewModelConsulta>>(lista);

            listaViewModel = Ordenar(listaViewModel);

            return Ok(listaViewModel);
        }

        /// <summary>
        /// Consulta um registro no repositório
        /// </summary>
        /// <returns>View model de consulta</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(IViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TViewModelCadastro>> ConsultarPorId(Guid id)
        {
            var entidade = await _crudService.ConsultarPorId(id);

            if (entidade == null)
                return CustomNotFound(nameof(id), "Registro não encontrado");

            var viewModel = _mapper.Map<TViewModelConsulta>(entidade);

            return Ok(viewModel);
        }


        /// <summary>
        /// Exclui uma entidade do repositório
        /// </summary>
        /// <param name="id">Id da entidade que será excluída</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> Delete(Guid id)
        {
            var entidade = await _crudService.ConsultarPorId(id);

            if (entidade == null)
                return CustomNotFound(nameof(id), "Registro não encontrado");

            await _crudService.Excluir(id);

            if (_crudService.Invalid)
                return BadRequest(_crudService.Notifications);

            await _crudService.Commit();

            return Ok();
        }

        /// <summary>
        /// Cadastra a entidade no repositório
        /// </summary>
        /// <param name="novaEntidadeViewModel">Dados da nova entidade</param>
        /// <returns>Id da entidade recém cadastrada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<Guid>> Post(TViewModelCadastro novaEntidadeViewModel)
        {
            var entidade = _mapper.Map<TEntity>(novaEntidadeViewModel);

            if (entidade.Invalid)
                return BadRequest(entidade.Notifications);

            await _crudService.Adicionar(entidade);

            if (_crudService.Invalid)
                return BadRequest(_crudService.Notifications);

            await _crudService.Commit();

            return Ok(new { entidade.Id });
        }

        /// <summary>
        /// Altera os dados de uma entidade do repositório
        /// </summary>
        /// <param name="id">Id da entidade que será alterada</param>
        /// <param name="viewModelCadastro">Dados que serão alterados</param>
        /// <returns>OK (201)</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> Put(Guid id, TViewModelCadastro viewModelCadastro)
        {
            var entidade = _mapper.Map<TEntity>(viewModelCadastro);

            if (entidade.Id != id)
                return CustomBadRequest(nameof(id), "Ids não conferem");

            await _crudService.Atualizar(entidade);

            if (_crudService.Invalid)
                return BadRequest(_crudService.Notifications);

            await _crudService.Commit();

            return Ok();
        }

        /// <summary>
        /// Método abstrato, no qual cada controller implementa a ordenação de forma customizada
        /// </summary>
        /// <param name="lista">Lista a ser ordenada</param>
        /// <returns>Lista já ordenada</returns>
        protected abstract IEnumerable<TViewModelConsulta> Ordenar(IEnumerable<TViewModelConsulta> lista);
    }
}
