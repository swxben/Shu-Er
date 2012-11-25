using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using ShellAndSettings.Dialogs;
using ShellAndSettings.Services;
using ShellAndSettings.Shell;

namespace ShellAndSettings
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        public AppBootstrapper()
        {
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
            AutoSubscribeEventAggegatorHandlers = true;
        }

        protected override void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            // Register modules in other assemblies:
            //builder.RegisterModule<ShellAndSettings.Services.ServicesModule>();

            builder.RegisterType<ShellViewModel>().AsSelf().As<IShell>().SingleInstance();
            builder
                .RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(t => t.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
                .AsSelf()
                .InstancePerDependency();
            builder
                .RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(t => t.Name.EndsWith("View"))
                .AsSelf()
                .InstancePerDependency();
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            builder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            base.ConfigureContainer(builder);
        }

        protected override void PrepareApplication()
        {
            Application.Startup += OnStartup;
            if (!Debugger.IsAttached) Application.DispatcherUnhandledException += OnUnhandledException;
            Application.Exit += OnExit;
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
        }

        protected override void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ShowDialog(e.Exception);
            }
            catch { }

            e.Handled = true;
        }

        static void ShowDialog(Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
            {
                inner = inner.InnerException;
            }

            ExceptionDialog dialog = new ExceptionDialog();
            dialog.Message = inner.Message;
            dialog.Details = ErrorMessageProducer.ExceptionToString(ex);
            dialog.Exception = ex;
            dialog.ShowDialog();
        }
    }
}
