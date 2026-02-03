using System.Globalization;
using MauiApp.Infrastructure.Models.DTO;

namespace MauiApp.Infrastructure.Models.Converters;

public class SelectedCorrectConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TaskForTest task)
            return false;

        var selected = task.AnswerVariantsWithFlag
            .FirstOrDefault(v => v.IsSelected);

        if (selected == null)
            return false;

        return selected.IsCorrect;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

}