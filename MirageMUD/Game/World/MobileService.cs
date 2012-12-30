using System;

namespace Mirage.Game.World
{
    public class MobileService : ServiceExecutorBase
    {
        private MudWorld _repository;

        public MobileService(MudWorld repository)
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
