using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Microsoft.RichPrompter
{
    public abstract class BaseViewModel
        : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var dependantProperties = Enumerable.Empty<IViewModelProperty>();
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                dependantProperties = dependants.Values.SelectMany(p => p);
            }
            else
            {
                if (dependants.TryGetValue(e.PropertyName, out ISet<IViewModelProperty> properties))
                {
                    dependantProperties = properties;
                }
            }
            foreach (var property in dependantProperties)
            {
                property.UpdateValue();
            }
            PropertyChanged?.Invoke(this, e);
        }
        private readonly IDictionary<string, ISet<IViewModelProperty>> dependants = new Dictionary<string, ISet<IViewModelProperty>>();

        protected ViewModelProperty<T> Property<T>(string propertyName) => new ViewModelProperty<T>(this, propertyName);
        protected ViewModelProperty<T> Property<T>(string propertyName, Func<T> getter, Action<T> setter = null) => new CustomViewModelProperty<T>(this, propertyName, getter, setter);

        protected void RefreshAllProperties() => OnPropertyChanged(allPropChangedEventArgs);
        private static readonly PropertyChangedEventArgs allPropChangedEventArgs = new PropertyChangedEventArgs(null);

        private interface IViewModelProperty
        {
            void UpdateValue();
        }

        protected class ViewModelProperty<T>
            : IViewModelProperty
        {
            public ViewModelProperty(BaseViewModel vm, string propertyName)
            {
                Debug.Assert(vm != null && !string.IsNullOrWhiteSpace(propertyName));
                viewModel = vm;
                e = new PropertyChangedEventArgs(propertyName);
            }

            public ViewModelProperty<T> DependOn<V>(ViewModelProperty<V> property)
            {
                if (property.viewModel.dependants.TryGetValue(property.e.PropertyName, out ISet<IViewModelProperty> properties))
                {
                    properties.Add(this);
                }
                else
                {
                    property.viewModel.dependants.Add(property.e.PropertyName, new HashSet<IViewModelProperty> { this });
                }
                return this;
            }

            public virtual void UpdateValue()
            {
            }

            protected virtual void UpdateField(T value)
            {
                if (!EqualityComparer<T>.Default.Equals(field, value))
                {
                    field = value;
                    viewModel.OnPropertyChanged(e);
                }
            }

            public T Value
            {
                get => field;
                set => UpdateField(value);
            }
            private T field;
            private readonly BaseViewModel viewModel;
            private readonly PropertyChangedEventArgs e;
            
        }

        protected class CustomViewModelProperty<T>
            : ViewModelProperty<T>
        {
            public CustomViewModelProperty(BaseViewModel vm, string propertyName, Func<T> getter, Action<T> setter = null)
                : base(vm, propertyName)
            {
                Debug.Assert(getter != null);
                this.getter = getter;
                this.setter = setter;
                UpdateValue();
            }

            public override void UpdateValue() => base.UpdateField(getter());

            protected override void UpdateField(T value)
            {
                setter?.Invoke(value);
                UpdateValue();
            }

            private readonly Func<T> getter;
            private readonly Action<T> setter;
        }


        protected ViewModelCommand<T> Command<T>(Action<T> execute) => new ViewModelCommand<T>(execute);

        protected class ViewModelCommand<T>
            : ICommand
        {
            public ViewModelCommand(Action<T> execute)
            {
                Debug.Assert(execute != null);
                this.execute = execute;
            }

            protected virtual void OnCanExecuteChanged(EventArgs e) => CanExecuteChanged?.Invoke(this, e);
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter) => execute((T)parameter);

            private readonly Action<T> execute;
        }
    }
    
}
