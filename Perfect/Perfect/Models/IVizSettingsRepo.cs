namespace DougKlassen.Revit.Perfect
{
    public interface IVizSettingsRepo
    {
        VizSettings LoadSettings();

        void WriteSettings(VizSettings settings);
    }
}
