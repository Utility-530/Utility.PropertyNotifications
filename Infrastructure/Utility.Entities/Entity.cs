using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using Utility.Interfaces.Generic.Data;

namespace Utility.Entities
{
    public class Entity : IId<Guid>, IIdSet<Guid>, INotifyPropertyChanged
    {
        [PrimaryKey]
        [Attributes.Column(ignore: true)]
        //[System.Text.Json.Serialization.JsonIgnore]
        //[Newtonsoft.Json.JsonIgnore]
        public Guid Id { get; set; }

        [System.ComponentModel.ReadOnly(true)]
        public DateTime Added { get; set; }

        [Utility.Attributes.Ignore]
        public DateTime Removed { get; set; }

        [System.ComponentModel.ReadOnly(true)]
        public DateTime LastUpdated { get; set; }

        #region propertyChanged

        /// <inheritdoc />
        /// <summary>
        ///     The event on property changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The caller member name of the property (auto-set)</param>
        //[NotifyPropertyChangedInvocator]
        public virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion propertyChanged
    }
}