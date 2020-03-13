namespace Api_CasaDeShow.Models
{
    public class Venda
    {
        public int Id {get; set;}
        public int Quantidade {get; set;}
        public float TotalCompra {get; set;}
        public Evento Evento {get; set;}        
    }
}