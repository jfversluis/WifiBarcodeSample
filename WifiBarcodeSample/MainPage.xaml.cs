using System;
using Xamarin.Forms;
using ZXing;

namespace WifiBarcodeSample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		public void Generate_Barcode(object sender, EventArgs e)
		{
			BarcodeScanView.IsVisible = false;
			BarcodeScanView.IsScanning = false;

			// TODO Implement error handling and validation
			var security = "";
			var ssidHidden = "";

			switch (Security.SelectedIndex)
			{
				case 0:
					security = "WPA";
					break;
				case 1:
					security = "WEP";
					break;
				default:
					security = "";
					break;
			}

			if (HiddenSsid.IsToggled)
				ssidHidden = "H:true";

			BarcodeImageView.BarcodeValue = $"WIFI:S:{Ssid.Text};T:{security};P:{Password.Text};{ssidHidden};";

			BarcodeImageView.IsVisible = true;
		}

		public void Scan_Barcode(object sender, EventArgs e)
		{
			BarcodeImageView.IsVisible = false;

			BarcodeScanView.IsVisible = true;

			BarcodeScanView.IsScanning = true;
		}

		public void Handle_OnScanResult(Result result)
		{
			if (string.IsNullOrWhiteSpace(result.Text))
				return;

			if (!result.Text.ToUpperInvariant().StartsWith("WIFI:", StringComparison.Ordinal))
				return;

			var ssid = GetValueForIdentifier('S', result.Text);
			var security = GetValueForIdentifier('T', result.Text);
			var password = GetValueForIdentifier('P', result.Text);
			var ssidHidden = GetValueForIdentifier('H', result.Text);

			Device.BeginInvokeOnMainThread(() =>
			{
				Ssid.Text = ssid;

				switch (security)
				{
					case "WPA":
						Security.SelectedIndex = 0;
						break;
					case "WEP":
						Security.SelectedIndex = 1;
						break;
					default:
						Security.SelectedIndex = 2;
						break;
				}

				Password.Text = password;
				HiddenSsid.IsToggled = !string.IsNullOrWhiteSpace(ssidHidden) && ssidHidden.ToUpperInvariant() == "TRUE";
			});
		}

		private string GetValueForIdentifier(char identifier, string haystack)
		{
			var startIdx = haystack.IndexOf($"{identifier}:", StringComparison.Ordinal);

			if (startIdx == -1)
				return "";

			startIdx += 2;

			var length = haystack.IndexOf(';', startIdx) - startIdx;

			return haystack.Substring(startIdx, length);
		}

		public void ShowHidePassword(object sender, EventArgs e)
		{
			Password.IsPassword = !Password.IsPassword;
		}
	}
}