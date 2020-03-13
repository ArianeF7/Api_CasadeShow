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
    [Route("api/casas")]
    [ApiController]
    [Authorize]
    //[Authorize(Policy = "Adm")]
    public class LocalController: ControllerBase
    {        
        private readonly ApplicationDbContext dataBase;
        public LocalController(ApplicationDbContext dataBase){
            this.dataBase = dataBase;
        }

        [HttpGet]
        public IActionResult GetLocais(){
            var casas = dataBase.Locais.ToList();
            //var listarLocais = dataBase.Locais.ToList();
            if(dataBase.Locais.Count() == 0){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem Casas de Show cadastradas!"});
            }else{
                Response.StatusCode = 200;
                return Ok(casas); 
            }          
        }              
        
        [HttpPost]
        public IActionResult Cadastrar([FromBody] LocalCasa LCasa){

            /*Validação das entradas*/
            if(LCasa.Nome == null && LCasa.Endereco == null){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Os campos são de preenchimento obrigatório"});
            }

            if(LCasa.Nome == null ||  LCasa.Nome == "" || LCasa.Endereco == null || LCasa.Endereco == ""){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Todos os campos são de preenchimento obrigatório"});
            }            
            
            if(dataBase.Locais.Any(achaLocal => achaLocal.Nome == LCasa.Nome)){
                Response.StatusCode = 409;
                return new ObjectResult(new {msg = "A casa de Show ja existe, tente outro nome"});
            }

            /*Cadastrando nova casa de show*/
            Local c =  new Local();

            c.Nome = LCasa.Nome;
            c.Endereco = LCasa.Endereco;
            dataBase.Locais.Add(c);
            dataBase.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(new {msg = "Casa de show cadastrada com sucesso"});            
        }

        [HttpGet("{id}")]
        public IActionResult GetLocais(int id){             
            try{
                var casas = dataBase.Locais.First(casa => casa.Id == id);
                return Ok(casas);
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O ID informado não existe"});            
            }            
        }

        [HttpPut("{Id}")]
        public IActionResult PatchEdit(int Id,[FromBody] Local LocalCasa){
            LocalCasa.Id = Id;
            if(LocalCasa.Id >0){

                try{
                    var LC = dataBase.Locais.First(c => c.Id == LocalCasa.Id);
                                       
                    //editar
                        LC.Nome = LocalCasa.Nome;
                        LC.Endereco = LocalCasa.Endereco;                        

                    if(LocalCasa.Nome != null && LocalCasa.Endereco != null && LocalCasa.Nome.Length > 0 && LocalCasa.Endereco.Length > 0 ){                            
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
        public IActionResult Deletar(int id){
            //Local local = dataBase.Locais.First(registro => registro.Id == id);            
            try{
                var casas = dataBase.Locais.First(casa => casa.Id == id);
                dataBase.Locais.Remove(casas);
                dataBase.SaveChanges();
                Response.StatusCode = 200;
                return new ObjectResult(new {msg = "Casa de show deletada com sucesso deletado com sucesso"});
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O ID informado não existe"});            
            }            
        }  

        [HttpGet("asc")]
        public IActionResult GetLocaisAlfab(){
            var casas = dataBase.Locais.OrderBy(alfab => alfab.Nome).ToList();
            //var listarLocais = dataBase.Locais.ToList();
            return Ok(casas);
        }

        [HttpGet("desc")]
        public IActionResult GetLocaisAlfab2(){
            var casas = dataBase.Locais.OrderByDescending(alfab => alfab.Nome).ToList();            
            //var listarLocais = dataBase.Locais.ToList();
            return Ok(casas);
        }

        [HttpGet("nome/{Nome}")]    
        public IActionResult GetLocaisNome(string Nome){           
            
            try{
                var casas = dataBase.Locais.First(casa => casa.Nome == Nome);
                return Ok(casas);
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O Nome informado não existe"});            
            }            
        }


        public class LocalCasa{
            public string Nome {get; set;}
            public string Endereco {get; set;}
        } 
    }
}