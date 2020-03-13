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
    [Route("api/Eventos")]
    [ApiController]
    [Authorize]
    public class EventoController: ControllerBase
    {
        private readonly ApplicationDbContext dataBase;
        public EventoController(ApplicationDbContext dataBase){
            this.dataBase = dataBase;
        }

        [HttpGet]
        public IActionResult GetEventos(){
            var listaLocais = dataBase.Locais.ToList();
            var listarEventos = dataBase.Eventos.ToList();
             if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(listarEventos); 
            }           
        }

        [HttpPost]
        public IActionResult CadastrarEvento([FromBody] EventoCasa ksa)
        {      
            /*Validação das entradas vazias*/
            if(ksa.Nome == null || ksa.Nome == "" || ksa.Local == null || ksa.Data == null || ksa.Capacidade < 1 || ksa.categoria == null ||
                 ksa.categoria == "" || ksa.preco <= 0 || ksa.Nome.Length < 1 || ksa.categoria.Length < 1 || ksa.Data < DateTime.Now){ 
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O campo não pode ser vazio "});            
            }

            if(ksa.Data < DateTime.Now){
                Response.StatusCode = 401;
                return new ObjectResult(new {msg = "A data não pode ser menor que a data de hoje"});
            }

            if(dataBase.Locais.Any(achaLocal => achaLocal.Id == ksa.Local.Id)){
                /*Cadastrando novo Evento*/
                Evento e = new Evento();

                e.Nome = ksa.Nome;
                e.Local = dataBase.Locais.First(nomeLocal => nomeLocal.Id == ksa.Local.Id);
                e.Data = ksa.Data;
                e.Capacidade = ksa.Capacidade;
                e.categoria = ksa.categoria;
                e.preco = ksa.preco;
                dataBase.Eventos.Add(e);
                dataBase.SaveChanges();           

                Response.StatusCode = 201;
                return new ObjectResult(new {msg = "Evento cadastrado com sucesso"});
            }else{
                 Response.StatusCode = 404;
                 return new ObjectResult(new {msg = "O Id inserido referente à casa de shows não existe"});
            }       
        }     

        [HttpGet("{id}")]
        public IActionResult GetEventos1(int id){
             try{
                var listaLocais = dataBase.Locais.ToList();
                var Eventos = dataBase.Eventos.First(evento => evento.Id == id);
                Response.StatusCode = 200;
                return Ok(Eventos);
             }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O ID informado não existe"});            
            }                
        }      

        [HttpPut("{Id}")]
        public IActionResult PutEdit(int Id,[FromBody] Evento EventoCasa){
            EventoCasa.Id = Id;
            if(EventoCasa.Id >0){

                try{
                    var EC = dataBase.Eventos.First(c => c.Id == EventoCasa.Id);

                    EC.Nome = EventoCasa.Nome;
                    EC.Local = dataBase.Locais.First(PegaLocal => PegaLocal.Id == EventoCasa.Local.Id);
                    EC.Data = EventoCasa.Data;
                    EC.Capacidade = EventoCasa.Capacidade;
                    EC.categoria = EventoCasa.categoria;
                    EC.preco = EventoCasa.preco;     

                    if(EventoCasa.Data < DateTime.Now){
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "A data deve ser maior ou igual a data atual"});
                    }                                       

                    if(EventoCasa.Nome != null && EventoCasa.Local != null && EventoCasa.Nome.Length > 0 && EventoCasa.Capacidade > 0 && EventoCasa.categoria != null
                        && EventoCasa.categoria.Length > 0 && EventoCasa.preco > 0 ){                            
                        dataBase.SaveChanges();
                        Response.StatusCode = 200;
                        return new ObjectResult(new {msg = "Alterações realizadas com sucesso"});                        
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Os campos são de preenchimento obrigatório!"});
                    }                   
                }catch{
                    Response.StatusCode = 404;
                    return new ObjectResult(new {msg = "O ID informado não foi encontrado"});
                }         

            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O ID informado é inválido!"});         
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarEvento(int id){
            //Local local = dataBase.Locais.First(registro => registro.Id == id);            
            
            try{
                var EventoC = dataBase.Eventos.First(eventCasa => eventCasa.Id == id);
                dataBase.Eventos.Remove(EventoC);
                dataBase.SaveChanges();
                Response.StatusCode = 200;
                return new ObjectResult(new {msg = "Evento deletado com sucesso"});
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O ID informado não existe"});            
            }            
        }  

        [HttpGet("capacidade/asc")]    
        public IActionResult GetEventosCapacAsc(){           
            var listaLocais = dataBase.Locais.ToList();
            var capacEvento = dataBase.Eventos.OrderBy(capacid => capacid.Capacidade).ToList();
             if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(capacEvento);  
            }                    
        }

        [HttpGet("capacidade/desc")]
        public IActionResult GetEventosCapacDesc(){
            var listaLocais = dataBase.Locais.ToList();
            var capacEvento = dataBase.Eventos.OrderByDescending(capacid => capacid.Capacidade).ToList();             
             if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(capacEvento);   
            }
        }

        [HttpGet("data/asc")]    
        public IActionResult GetEventosDataAsc(){     
            var listaLocais = dataBase.Locais.ToList();            
            var dataEvento = dataBase.Eventos.OrderBy(date => date.Data).ToList();
            if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(dataEvento);   
            }                    
        }

         [HttpGet("data/desc")]    
        public IActionResult GetEventosDataDesc(){    
            var listaLocais = dataBase.Locais.ToList();            
            var dataEvento = dataBase.Eventos.OrderByDescending(date => date.Data).ToList();
            if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(dataEvento);   
            }                      
        }

        [HttpGet("nome/asc")]    
        public IActionResult GetEventosNomeAsc(){      
            var listaLocais = dataBase.Locais.ToList();            
            var nomeEvento = dataBase.Eventos.OrderBy(name => name.Nome).ToList();
             if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(nomeEvento);  
            }                        
        }

        [HttpGet("nome/desc")]    
        public IActionResult GetEventosNomeDesc(){  
            var listaLocais = dataBase.Locais.ToList();          
            var nomeEvento = dataBase.Eventos.OrderByDescending(name => name.Nome).ToList();
            
            if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(nomeEvento);  
            }                     
        }

         [HttpGet("preco/asc")]    
        public IActionResult GetEventosPrecoAsc(){   
            var listaLocais = dataBase.Locais.ToList();            
            var precoEvento = dataBase.Eventos.OrderBy(name => name.preco).ToList();

            if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(precoEvento);     
            }                                        
        }

        [HttpGet("preco/desc")]    
        public IActionResult GetEventosPrecoDesc(){   
            var listaLocais = dataBase.Locais.ToList();            
            var precoEvento = dataBase.Eventos.OrderByDescending(name => name.preco).ToList();

            if(dataBase.Eventos.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem eventos cadastrados!"});
            }else{
                Response.StatusCode = 200;
                return Ok(precoEvento);    
            }                
        }

        public class EventoCasa{     
            public string Nome { get; set; }
            public int Capacidade { get; set; }
            public float preco { get; set; }        
            public string categoria { get; set; }        
            public DateTime Data { get; set; }        
            public Local Local { get; set; }
        }
    }
}