using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.Models;
using Entities.Services;

namespace Manager
{
    public class PromocoesManager
    {
        public static void Processar(int mensagem, DateTime vencimento)
        {
            PromocoesService service = new PromocoesService();
            ICollection<Promocoes> promocoes = service.Consult(mensagem.ToString(), "Mensagem");
            int count = promocoes.Count;

            Promocoes entity;

            if (count > 0)
            {
                entity = promocoes.First();
                entity.Vencimento = vencimento;
                entity.Updated = DateTime.Now;

                service.Edit(entity);

                return;
            }

            entity = new Promocoes();
            entity.SetParams(mensagem, vencimento, 1);
            entity.Created = DateTime.Now;

            service.Add(entity);
        }
       
    }
}