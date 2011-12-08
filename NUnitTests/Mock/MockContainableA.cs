using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.World.Containers;

namespace NUnitTests.Mock
{
    public class MockContainableA : IContainable
    {
        private IContainer _container;
        #region IContainable Members

        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public bool CanBeContainedBy(IContainer container)
        {
            return true;
        }

        #endregion
    }

    public class MockContainableB : IContainable
    {
        private IContainer _container;
        #region IContainable Members

        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public bool CanBeContainedBy(IContainer container)
        {
            return true;
        }

        #endregion
    }

    public class MockContainableSubA : MockContainableA
    {
    }

    public class MockContainableD : IContainable
    {
        private IContainer _container;
        #region IContainable Members

        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public bool CanBeContainedBy(IContainer container)
        {
            return true;
        }

        #endregion
    }

}
