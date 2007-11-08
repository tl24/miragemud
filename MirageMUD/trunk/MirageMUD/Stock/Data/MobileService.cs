using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;

namespace Mirage.Stock.Data
{
    public class MobileService : ServiceExecutorBase
    {
        private StockRepository _repository;

        public MobileService(StockRepository repository)
        {
            _repository = repository;
        }

        public override ServiceMethod GetServiceMethod(string key)
        {
            switch (key.ToLower())
            {
                case "processinput":
                    return ProcessInput;
                default:
                    throw new ArgumentException("There is no service method associated with the key: " + key);
            }
        }

        public void ProcessInput()
        {
            foreach (Mobile mob in _repository.Mobiles)
            {
                mob.ProcessInput();
            }
        }

    }
}
