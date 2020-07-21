using DDS.WebAPI.Abstractions.ViewModels;

namespace DDS.WebAPI.Models.ViewModels
{
    /// <summary>
    /// Classe que serve para enviar uma string por post
    /// </summary>
    public class StringContainerViewModel : IViewModel
    {
        /// <summary>
        /// Texto a ser enviado
        /// </summary>
        public string Texto { get; set; }
    }
}
