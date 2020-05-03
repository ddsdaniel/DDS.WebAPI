﻿using Flunt.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DDS.WebAPI.Abstractions.Controllers
{
    /// <summary>
    /// Controller genérica que possui métodos úteis à outras controllers. Todas as controllers do sistema deve herdar desta
    /// </summary>
    [Authorize]
    public abstract class GenericController : ControllerBase
    {
        /// <summary>
        /// Monta um retorno de not found com um conteúdo formatado como uma lista de notificações, para ficar compatível com o padrão de retorno de erros do sistema
        /// </summary>
        /// <param name="property">Nome da propriedade do erro</param>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>NotFoundObjectResult com uma lista de notificações de apenas um registro, o qual foi passado por parâmetro</returns>
        protected virtual NotFoundObjectResult CustomNotFound(string property, string message)
            => NotFound(new List<Notification> { new Notification(property, message) });

        /// <summary>
        /// Monta um retorno de bad request com um conteúdo formatado como uma lista de notificações, para ficar compatível com o padrão de retorno de erros do sistema
        /// </summary>
        /// <param name="property">Nome da propriedade do erro</param>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>BadRequestObjectResult com uma lista de notificações de apenas um registro, o qual foi passado por parâmetro</returns>
        protected virtual BadRequestObjectResult CustomBadRequest(string property, string message)
            => BadRequest(new List<Notification> { new Notification(property, message) });
    }
}
