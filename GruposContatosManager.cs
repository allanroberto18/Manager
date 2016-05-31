using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models;
using Entities.Services;

namespace Manager
{
    public class GruposContatosManager
    {
        public static IEnumerable<GruposContatos> CheckGruposContatos(int grupo, int contato)
        {
            GruposContatosService services = new GruposContatosService();
            return services.List().Where(i => i.GrupoId.Equals(grupo) && i.ContatoId.Equals(contato)).ToList();
        }

        public static GruposContatos ProcessarGruposContatos(int contato, int grupo)
        {
            GruposContatos entity;

            IEnumerable<GruposContatos> checkGruposContatos = CheckGruposContatos(grupo, contato);
            int countGrupoContato = checkGruposContatos.Count();
            if (countGrupoContato > 0)
            {
                GruposContatos vo = checkGruposContatos.First();
                return vo;
            }
            else
            {
                entity = new GruposContatos();
                entity.SetParams(grupo, contato, 1);
                entity.Created = DateTime.Now;

                GruposContatosService services = new GruposContatosService();
                services.Add(entity);
            }
            return entity;
        }
    }
}