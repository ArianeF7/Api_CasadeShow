using System;

namespace Api_CasaDeShow.Models
{
    public class Evento
    {
        public int Id { get; set; }      
        public string Nome { get; set; }
        public int Capacidade { get; set; }
        public float preco { get; set; }        
        public string categoria { get; set; }        
        public DateTime Data { get; set; }        
        public Local Local { get; set; }
    }
}