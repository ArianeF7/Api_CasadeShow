using System;
using System.Collections.Generic;
using System.Linq;
using Api_CasaDeShow.Data;
using Api_CasaDeShow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_CasaDeShow.Controllers
{

    [Route("Api/vendas")]
    [ApiController]
    [Authorize]
    public class VendaController: ControllerBase
    {
        private readonly ApplicationDbContext dataBase;
        public VendaController(ApplicationDbContext dataBase)
        {
            this.dataBase = dataBase;
        }
  

         [HttpPost]
        public IActionResult CadastrarVendas([FromBody] VendaCasa vend)
        {      
            /*Validação das entradas vazias*/
            if(vend.Quantidade <= 1){
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Os campos são de preenchimento Obrigatório"});
            }            

            if(dataBase.Eventos.Any(achaEvento => achaEvento.Id == vend.Evento.Id)){
                /*Cadastrando novo Evento*/
                Venda v = new Venda();

                v.Quantidade = vend.Quantidade;
                v.Evento = dataBase.Eventos.First(pegaE => pegaE.Id == vend.Evento.Id);

                v.Evento.Capacidade = v.Evento.Capacidade - vend.Quantidade; 
                v.TotalCompra = v.Evento.preco * vend.Quantidade;

                dataBase.Vendas.Add(v);
                dataBase.SaveChanges();           

                Response.StatusCode = 201;
                return new ObjectResult(new {msg = "Venda cadastrado com sucesso"});
            }else{
                 Response.StatusCode = 404;
                 return new ObjectResult(new {msg = "O Id inserido referente à casa de shows não existe"});
            }       
        }     

        [HttpGet]
        public IActionResult GetVenda(){           
             if(dataBase.Vendas.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem vendas para esse usuário!"});
            }else{                
                var listaLocais = dataBase.Locais.ToList();
                var listaEventos = dataBase.Eventos.ToList();
                var ListaVendas = dataBase.Vendas.ToList();
                
                Response.StatusCode = 200;
                return Ok(ListaVendas); 
            }           
        }

        [HttpGet("{id}")]
        public IActionResult GetVenda(int id){             
            try{
                var listaLocais = dataBase.Locais.ToList();
                var listaEventos = dataBase.Eventos.ToList();
                var ListaVendas = dataBase.Vendas.ToList();
                var venda = dataBase.Vendas.First(sale => sale.Id == id);              
                return Ok(venda);
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O ID informado não existe"});            
            }            
        }
        
        public class VendaCasa
        {
            public int Quantidade {get; set;}           
            public Evento Evento {get; set;}
            public float TotalCompra {get; set;}   
        }


    }
}