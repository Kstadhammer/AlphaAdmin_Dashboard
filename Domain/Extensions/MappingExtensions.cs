using System;
using System.Linq;
using System.Reflection;

namespace Domain.Extensions;

public static class MappingExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        TDestination destination = Activator.CreateInstance<TDestination>()!;

        // Special case for UserEntity creation
        // Special case for MemberEntity creation from SignUpFormData
        if (
            typeof(TDestination).Name == "MemberEntity"
            && source.GetType().Name == "SignUpFormData"
        )
        {
            // Get Email property
            var emailProperty = source.GetType().GetProperty("Email");
            var emailValue = emailProperty?.GetValue(source)?.ToString();

            if (emailValue != null)
            {
                // Set both Email and UserName to the same value
                var destEmailProperty = typeof(TDestination).GetProperty("Email");
                var destUserNameProperty = typeof(TDestination).GetProperty("UserName");

                destEmailProperty?.SetValue(destination, emailValue);
                destUserNameProperty?.SetValue(destination, emailValue); // Required for ASP.NET Identity
            }
        }

        var sourceProperties = source
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destination
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destinationProperty in destinationProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(x =>
                x.Name == destinationProperty.Name
                && x.PropertyType == destinationProperty.PropertyType
            );
            if (sourceProperty != null && destinationProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source);
                destinationProperty.SetValue(destination, value);
            }
        }

        return destination;
    }
}
