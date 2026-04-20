using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace ThymeToPlant.Services;

public class LocationZipService
{
    public virtual async Task<LocationZipResult> GetCurrentZipAsync()
    {
        var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (permissionStatus != PermissionStatus.Granted)
        {
            permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (permissionStatus != PermissionStatus.Granted)
        {
            return LocationZipResult.Failure("Location permission denied. Please enter a ZIP code.");
        }

        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);
            if (location is null)
            {
                return LocationZipResult.Failure("Unable to determine your location.");
            }

            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var zipCode = placemarks?.FirstOrDefault(static placemark => !string.IsNullOrWhiteSpace(placemark.PostalCode))?.PostalCode;
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                return LocationZipResult.Failure("Unable to determine ZIP code from your location.");
            }

            return LocationZipResult.Success(zipCode);
        }
        catch (FeatureNotSupportedException)
        {
            return LocationZipResult.Failure("Location is not supported on this device.");
        }
        catch (FeatureNotEnabledException)
        {
            return LocationZipResult.Failure("Location services are disabled.");
        }
        catch (PermissionException)
        {
            return LocationZipResult.Failure("Location permission denied. Please enter a ZIP code.");
        }
        catch
        {
            return LocationZipResult.Failure("Unable to use your location right now.");
        }
    }
}

public sealed class LocationZipResult
{
    private LocationZipResult(bool isSuccess, string zipCode, string errorMessage)
    {
        IsSuccess = isSuccess;
        ZipCode = zipCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public string ZipCode { get; }
    public string ErrorMessage { get; }

    public static LocationZipResult Success(string zipCode) => new(true, zipCode, string.Empty);

    public static LocationZipResult Failure(string errorMessage) => new(false, string.Empty, errorMessage);
}
