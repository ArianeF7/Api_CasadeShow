using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Api_CasaDeShow.Data;
using Api_CasaDeShow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Api_CasaDeShow.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext dataBase;
        public UsuarioController(ApplicationDbContext dataBase){
            this.dataBase = dataBase;
        }

        [HttpPost]
        public IActionResult PostUsuario([FromBody] Usuario usuarioCasa){
            if(usuarioCasa.Email == null && usuarioCasa.Senha == null){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Os campos são de preenchimento obrigatório"});
            }

            if(usuarioCasa.Email == null || usuarioCasa.Senha == null || usuarioCasa.Email == "" || usuarioCasa.Senha == ""){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O campo não pode ser vazio"});
            }

            if(dataBase.Usuarios.Any(achaUser => achaUser.Email == usuarioCasa.Email)){
                Response.StatusCode = 409;
                return new ObjectResult(new {msg = "O email informado já existe"});
            } 

            // if(usuarioCasa.TipoUsuario < 1 || usuarioCasa.TipoUsuario > 2){
            //     Response.StatusCode = 409;
            //     return new ObjectResult(new {msg = "tipo de usuário incorreto, por gentileza coloque 1 para selecionar administrador ou 2 para selecionar usuário"});
            // }

            if(usuarioCasa.Senha.Length < 3){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "A senha deve conter mais que 3 caracteres"});
            }
            
            Usuario u =  new Usuario();

            u.Email = usuarioCasa.Email;
            u.Senha = usuarioCasa.Senha;
            // u.TipoUsuario = usuarioCasa.TipoUsuario;          
            dataBase.Usuarios.Add(u);
            dataBase.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult(new {msg = "Usuário cadastrado com sucesso"});           
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Usuario credenciais){
            //buscar um usuário por e-mail
            //verificar a senha correta
            //gerar um token JWT e retornar o token para o usuario 
                       
            try{
                Usuario UserValido = dataBase.Usuarios.First(user => user.Email.Equals(credenciais.Email));
            
                if(UserValido != null){
                    if(UserValido.Senha.Equals(credenciais.Senha)){
                        //Usuario acertou a senha => gera o token e logou]                       

                        string chaveDeSeguranca = "chave_de_segurança_api";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var JWT = new JwtSecurityToken(
                            issuer: "Api_CasadeShow", //fornecedor do JWT
                            expires: DateTime.Now.AddHours(1),
                            audience: "Usuário Geral",
                            signingCredentials: credenciaisDeAcesso
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));

                    }else{
                        Response.StatusCode = 401;
                        return new ObjectResult(new {msg = "Usuário inválido, tente novamente"});
                    }

                }else{
                    Response.StatusCode = 401;
                    return new ObjectResult(new {msg = "Usuário inválido, tente novamente"});
                }
            }catch(Exception){
                Response.StatusCode = 401;
                return new ObjectResult(new {msg = "Usuário inválido, tente novamente"});
                
            }

      

        }

        [HttpGet]
        public IActionResult GetUsuarios(){
            try{
                if(dataBase.Usuarios.Count() == 0){
                    Response.StatusCode = 404;
                    return new ObjectResult(new {msg = "não existe Usuários Cadastrados"});
                }

                List<UsuarioCasa> User1 = dataBase.Usuarios.Select(ApresentaUser => new UsuarioCasa(ApresentaUser)).ToList();
                return Ok(User1);
            }catch{
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "Não existem usuários cadastrados"});    
            }
            //var users = dataBase.Usuarios.ToList();
            //var listarLocais = dataBase.Locais.ToList();
            
        }

    

        [HttpGet("{id}")]
        public IActionResult GetUsuarios(int id){             
            try{
                var users = dataBase.Usuarios.First(user => user.Id == id);
                UsuarioCasa User1 = new UsuarioCasa(users); 
                return Ok(User1);
            }catch(Exception){
                Response.StatusCode = 404;
                return new ObjectResult(new {msg = "O ID informado não existe"});            
            }            
        }

        
        public class UsuarioCasa
        {

            public UsuarioCasa(Usuario usuario1)
            {
                Id = usuario1.Id;
                Email = usuario1.Email;
            }

            public int Id {get; set;}
            public string Email { get; set; }
            // public string Senha { get; set; }            
        }
    }
}