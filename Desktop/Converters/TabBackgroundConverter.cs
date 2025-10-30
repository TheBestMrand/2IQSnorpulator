using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Desktop.Converters;

public class TabBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var selectedTab = value as string;
        var targetTab = parameter as string;
        
        return selectedTab == targetTab 
            ? new SolidColorBrush(Color.Parse("#0a0a0a")) 
            : new SolidColorBrush(Color.Parse("#000000"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TabBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var selectedTab = value as string;
        var targetTab = parameter as string;
        
        return selectedTab == targetTab 
            ? new SolidColorBrush(Color.Parse("#ffffff")) 
            : new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TabForegroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var selectedTab = value as string;
        var targetTab = parameter as string;
        
        return selectedTab == targetTab 
            ? new SolidColorBrush(Color.Parse("#ffffff")) 
            : new SolidColorBrush(Color.Parse("#888888"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ActiveTabBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isActive = value is bool active && active;
        
        return isActive 
            ? new SolidColorBrush(Color.Parse("#0a0a0a")) 
            : new SolidColorBrush(Color.Parse("#000000"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ActiveTabForegroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isActive = value is bool active && active;
        
        return isActive 
            ? new SolidColorBrush(Color.Parse("#ffffff")) 
            : new SolidColorBrush(Color.Parse("#888888"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TabVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var selectedTab = value as string;
        var targetTab = parameter as string;
        
        return selectedTab == targetTab;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SendButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isSending = value is bool sending && sending;
        return isSending ? "SENDING..." : "SEND";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BodyTypeVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var bodyType = value as string;
        return bodyType != "None";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class LoadFullResponseTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isLoading = value is bool loading && loading;
        return isLoading ? "Loading..." : "Load Full Response";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}