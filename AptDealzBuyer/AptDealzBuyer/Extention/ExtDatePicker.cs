using AptDealzBuyer.Utility;
using System;
using Xamarin.Forms;

namespace AptDealzBuyer.Extention
{
    public class ExtDatePicker : Xamarin.Forms.DatePicker
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public const string NullableDatePropertyName = "NullableDate";

        // Die BinableProperty
        public static readonly BindableProperty NullableDateProperty = BindableProperty.Create<ExtDatePicker, DateTime?>(i => i.NullableDate, null, BindingMode.TwoWay, null, NullableDateChanged);

        // Datumswert welches null Werte akzeptiert
        public DateTime? NullableDate
        {
            get
            {
                return (DateTime?)this.GetValue(NullableDateProperty);
            }
            set
            {
                this.SetValue(NullableDateProperty, value);
            }
        }

        // Der Name der<c> NullText</c> Property
        public const string NullTextPropertyName = "NullText";

        // Die BindableProperty
        public static readonly BindableProperty NullTextProperty = BindableProperty.Create<ExtDatePicker, string>(i => i.NullText, default(string), BindingMode.TwoWay);

        // Der Text der angezeigt wird wenn<c>NullableDate</c> keinen Wert hat
        public string NullText
        {
            get
            {
                return (string)this.GetValue(NullTextProperty);
            }
            set
            {
                this.SetValue(NullTextProperty, value);
            }
        }

        // Der Name der<c> DisplayBorder</c> Property
        public const string DisplayBorderPropertyName = "DisplayBorder";

        // Die BindableProperty
        public static readonly BindableProperty DisplayBorderProperty = BindableProperty.Create<ExtDatePicker, bool>(i => i.DisplayBorder, default(bool), BindingMode.TwoWay);

        // Gibt an ob eine Umrandung angezeigt werden soll oder nicht
        public bool DisplayBorder
        {
            get
            {
                return (bool)this.GetValue(DisplayBorderProperty);
            }
            set
            {
                this.SetValue(DisplayBorderProperty, value);
            }
        }


        // Erstellt eine neue Instanz von<c> CustomDatePicker</c>
        public ExtDatePicker()
        {
            this.DateSelected += CustomDatePicker_DateSelected;
            // this.Date = DateTime.Now.Date;
            this.Format = Constraints.Str_DateFormate;
        }

        // Wird gefeuert wenn ein neues Datum selektiert wurde
        // <param name = "sender" > Der Sender</param>
        // <param name = "e" > Event Argumente</param>
        void CustomDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            this.NullableDate = new DateTime(
                    e.NewDate.Year,
                    e.NewDate.Month,
                    e.NewDate.Day,
                    this.NullableDate.HasValue ? this.NullableDate.Value.Hour : 0,
                    this.NullableDate.HasValue ? this.NullableDate.Value.Minute : 0,
                    this.NullableDate.HasValue ? this.NullableDate.Value.Second : 0);
        }

        // Gefeuert wenn sich<c> NullableDate</c> ändert
        // <param name = "obj" > Der Sender</param>
        // <param name = "oldValue" > Der alte Wert</param>
        // <param name = "newValue" > Der neue Wert</param>
        private static void NullableDateChanged(BindableObject obj, DateTime? oldValue, DateTime? newValue)
        {
            var customDatePicker = obj as ExtDatePicker;

            if (customDatePicker != null)
            {
                if (newValue.HasValue)
                {
                    customDatePicker.Date = newValue.Value;
                }
            }
        }

        public string Placeholder
        {
            get;
            set;
        }
#pragma warning restore CS0618 // Type or member is obsolete

    }
}
