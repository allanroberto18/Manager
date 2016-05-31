using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models;
using Entities.Services;

namespace Manager
{
    public class GruposManager
    {
        public static Grupos ProcessarGrupos(string nome)
        {
            Grupos entity;

            IEnumerable<Grupos> checkGrupos = ConsultByNomeEqual(nome.ToUpper());
            int countGrupos = checkGrupos.Count();

            if (countGrupos > 0)
            {
                Grupos vo = checkGrupos.First();
                return vo;
            }
            else
            {
                entity = new Grupos();
                entity.SetParams(nome, 1);
                entity.Created = DateTime.Now;
                
                GruposService services = new GruposService();
                services.Add(entity);
            }

            return entity;
        }

        public static IEnumerable<Grupos> ConsultByNomeEqual(string nome)
        {
            GruposService services = new GruposService();
            return services.List().Where(i => i.Nome.Equals(nome)).ToList();
        }
    }
}