using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternationalWaterWebApp.Library.ViewModel
{
    internal class CommonEnums
    {
    }

    public enum StatusEnum
    {
        Yes = 0,
        No = 1
    }

    public enum UserTypeEnum
    {
        Admin = 1,
        Agronomist = 2,
        Producer = 3
    }
    public enum ValidationMessages
    {
        [Description("Record saved successfully.")]
        SavedSuccessfully,
        [Description("Record updated successfully.")]
        UpdatedSuccessfully,
        [Description("The advisor saved successfully.")]
        AdvisorSavedSuccessfully,
        [Description("The producer saved successfully.")]
        ProducerSavedSuccessfully,
        [Description("Email Id already exists. Please enter a new one.")]
        EmailAlreadyExists,
        [Description("Please enter the proper values.")]
        ProperValues,
        [Description("No Data Found.")]
        NoDataFound,
        [Description("The zone saved successfully.")]
        ZoneSavedSuccessfully
    }

}