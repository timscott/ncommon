using Autofac;
using Microsoft.Practices.ServiceLocation;
using NCommon.ContainerAdapter.Autofac;
using NCommon.Data;
using NCommon.Data.NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.ContainerAdapters.Tests.Autofac
{
    [TestFixture]
    public class when_configuring_data_using_NHConfiguration
    {
        IContainer _container;

        [TestFixtureSetUp()]
        public void FixtureSetup()
        {
            var builder = new ContainerBuilder();
            NCommon.Configure
                .Using(new AutofacContainerAdapter(builder))
                .ConfigureData<NHConfiguration>();
            ServiceLocator.SetLocatorProvider(() => MockRepository.GenerateStub<IServiceLocator>());
            _container = builder.Build();
        }

        [Test]
        public void verify_IUnitOfWorkFactory_is_NHUnitOfWorkFactory()
        {
            var factory = _container.Resolve<IUnitOfWorkFactory>();
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.TypeOf<NHUnitOfWorkFactory>());
        }

        [Test]
        public void verify_NHUnitOfWorkFactory_is_registered_as_singleton()
        {
            var factory1 = _container.Resolve<IUnitOfWorkFactory>();
            var factory2 = _container.Resolve<IUnitOfWorkFactory>();
            Assert.That(factory1, Is.Not.Null);
            Assert.That(factory2, Is.Not.Null);
            Assert.That(factory1, Is.SameAs(factory2));
        }

        [Test]
        public void verify_can_get_instances_of_IRepository()
        {
            var repo = _container.Resolve<IRepository<string>>();
            Assert.That(repo, Is.Not.Null);
            Assert.That(repo, Is.TypeOf(typeof(NHRepository<string>)));
        }

        [Test]
        public void verify_returns_distinct_configured_repository()
        {
            var builder = new ContainerBuilder();
            NCommon.Configure
                .Using(new AutofacContainerAdapter(builder))
                .ConfigureData<NHConfiguration>(x => x.WithDistinctResults());
            var container = builder.Build();

            var repo = container.Resolve<IRepository<string>>();
            Assert.That(repo, Is.Not.Null);
            Assert.That(repo, Is.TypeOf<NHRepository<string>.WithDistinctRoot>());
        }
    }
}