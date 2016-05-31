using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models;
using Entities.Services;

namespace Manager
{
    public class ContatosManager
    {
        public static Contatos ProcessarContatos(string nome, int sexo, string telefone, string dataNascimento)
        {
            Contatos entity;

            IEnumerable<Contatos> checkContato = GetContatosByNomeAndTelefone(nome, telefone);
            int countContatos = checkContato.Count();
            
            if (countContatos > 0)
            {
                Contatos vo = checkContato.First();
                return vo;
            }
            
            entity = new Contatos();

            DateTime param;
            bool testeData = DateTime.TryParse(dataNascimento, out param);
            if (testeData)
            {
                entity.SetParams(nome, sexo, telefone, param, 1);
            }
            else
            {
                entity.SetParams(nome, sexo, telefone, 1);
            }
            entity.Created = DateTime.Now;

            ContatosService services = new ContatosService();
            services.Add(entity);
            
            return entity;
        }

        public static IEnumerable<Contatos> GetContatosByNomeAndTelefone(string nome, string telefone)
        {
            ContatosService services = new ContatosService();
            return services.List().Where(i => i.Nome.Equals(nome) && i.Telefone.Equals(telefone)).ToList();
        }
    }
}