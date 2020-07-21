using DDS.WebAPI.Abstractions.ViewModels;

namespace DDS.WebAPI.Models.ViewModels
{
    /// <summary>
    /// Classe usada para filtros em CRUDs
    /// </summary>
    public class FiltroViewModel : IViewModel
    {
        /// <summary>
        /// Filtro inserido pelo usuário
        /// </summary>
        public string Filtro { get; set; }
    }
}
