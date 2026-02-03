using System.Globalization;
using MauiApp.Infrastructure.Models.DTO;

namespace MauiApp.Infrastructure.Models.Converters;

public class HasSelectedAnswerConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TaskForTest task)
            return false;

        return task.AnswerVariantsWithFlag.Any(v => v.IsSelected);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
