using System;
using System.Linq;
using System.Reflection;

namespace Domain.Extensions;

/// <summary>
/// Provides extension methods for object mapping.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Had help from AI to fix errors and refactor the code
    /// Maps properties from a source object to a new destination object of type <typeparamref name="TDestination"/>.
    /// Copies properties with matching names and types.
    /// Includes special handling for mapping <c>SignUpFormData</c> to <c>MemberEntity</c> to set both Email and UserName.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination object to create.</typeparam>
    /// <param name="source">The source object whose properties will be copied.</param>
    /// <returns>A new object of type <typeparamref name="TDestination"/> with mapped properties.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source object is null.</exception>
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
