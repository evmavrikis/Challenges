using System;
using System.Collections.Generic;
using System.Linq;
using VolatilityContracts;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VolatilityWPFApp.ViewModels
{
    public class CustomerDetailsView : INotifyPropertyChanged
    {
        private static List<TitleView> _titles;
        private static List<GenderView> _genders;
        public event PropertyChangedEventHandler PropertyChanged;
        public CustomerDetails CustomerDetails { get; set; } = new CustomerDetails();

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string FirstName
        {
            get
            {
                return CustomerDetails.FirstName;
            }
            set
            {
                CustomerDetails.FirstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get
            {
                return CustomerDetails.LastName;
            }
            set
            {
                CustomerDetails.LastName = value;
                OnPropertyChanged();
            }
        }

        public Title Title
        {
            get
            {
                return CustomerDetails.Title;
            }
            set
            {
                CustomerDetails.Title = value;
                OnPropertyChanged();
            }
        }

        public Gender Gender
        {
            get
            {
                return CustomerDetails.Gender;
            }
            set
            {
                CustomerDetails.Gender = value;
                OnPropertyChanged();
            }
        }

        public string ContactNumber
        {
            get
            {
                return CustomerDetails.ContactNumber;
            }
            set
            {
                CustomerDetails.ContactNumber = value;
                OnPropertyChanged();
            }
        }

        public DateTime DOB
        {
            get
            {
                return CustomerDetails.DOB;
            }
            set
            {
                CustomerDetails.DOB = value;
                OnPropertyChanged();
            }
        }

        public string OtherNames
        {
            get
            {
                return CustomerDetails.OtherNames;
            }
            set
            {
                CustomerDetails.OtherNames = value;
                OnPropertyChanged();
            }
        }

        public string EmailAddress
        {
            get
            {
                return CustomerDetails.EmailAddress;
            }
            set
            {
                CustomerDetails.EmailAddress = value;
                OnPropertyChanged();
            }
        }

        public string PostalAddressLine1
        {
            get
            {
                return CustomerDetails.PostalAddressLine1;
            }
            set
            {
                CustomerDetails.PostalAddressLine1 = value;
                OnPropertyChanged();
            }
        }

        public string PostalAddressLine2
        {
            get
            {
                return CustomerDetails.PostalAddressLine2;
            }
            set
            {
                CustomerDetails.PostalAddressLine2 = value;
                OnPropertyChanged();
            }
        }

        public string PostCode
        {
            get
            {
                return CustomerDetails.PostCode;
            }
            set
            {
                CustomerDetails.PostCode = value;
                OnPropertyChanged();
            }
        }
        public List<TitleView> Titles
        {
            get
            {
                if (_titles == null)
                {
                    _titles = new List<TitleView>();
                    var titles = Enum.GetValues(typeof(Title)).Cast<Title>();
                    foreach (var t in titles)
                    {
                        var tv = new TitleView()
                        {
                            Id = t,
                            Display = t.ToString()
                        };
                        _titles.Add(tv);
                    }
                }
                return _titles;
            }
            set
            {

            }
        }

        public List<GenderView> Genders
        {
            get
            {
                if (_genders == null)
                {
                    _genders = new List<GenderView>();
                    var gendres = Enum.GetValues(typeof(Gender)).Cast<Gender>();
                    foreach (var t in gendres)
                    {
                        var tv = new GenderView()
                        {
                            Id = t,
                            Display = t.ToString()
                        };
                        _genders.Add(tv);
                    }
                }
                return _genders;
            }
            set
            {

            }
        }
    }
}
