using System;
using System.Collections.Generic;
using System.Linq;
using VolatilityContracts;

namespace VolatilityWPFApp.ViewModels
{
    public class CustomerDetailsView
    {
        private static List<TitleView> _titles;
        private static List<GenderView> _genders;

        public CustomerDetails CustomerDetails { get; set; } = new CustomerDetails();
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
