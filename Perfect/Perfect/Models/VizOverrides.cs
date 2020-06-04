using Autodesk.Revit.DB;
using System;

namespace DougKlassen.Revit.Perfect
{
    public class VizOverrides
    {
        //public Int32 DetailLevel { get; set; }
        public Boolean Halftone { get; set; }
        public Int32 ProjectionLinePatternId { get; set; }
        public VizColor ProjectionLineColor { get; set; }
        public Int32 ProjectionLineWeight { get; set; }
        public Boolean IsSurfaceForegroundPatternVisible { get; set; }
        public Int32 SurfaceForegroundPatternId { get; set; }
        public VizColor SurfaceForegroundPatternColor { get; set; }
        public Boolean IsSurfaceBackgroundPatternVisible { get; set; }
        public Int32 SurfaceBackgroundPatternId { get; set; }
        public VizColor SurfaceBackgroundPatternColor { get; set; }
        public Int32 SurfaceTransparency { get; set; }
        public Int32 CutLinePatternId { get; set; }
        public VizColor CutLineColor { get; set; }
        public Int32 CutLineWeight { get; set; }
        public Boolean IsCutForegroundPatternVisible { get; set; }
        public Int32 CutForegroundPatternId { get; set; }
        public VizColor CutForegroundPatternColor { get; set; }
        public Boolean IsCutBackgroundPatternVisible { get; set; }
        public Int32 CutBackgroundPatternId { get; set; }
        public VizColor CutBackgroundPatternColor { get; set; }

        public VizOverrides()
        {
            OverrideGraphicSettings settings = new OverrideGraphicSettings();
            AssignValues(settings);
        }

        public VizOverrides(OverrideGraphicSettings settings)
        {
            AssignValues(settings);
        }

        private void AssignValues(OverrideGraphicSettings settings)
        {
            //DetailLevel = (Int32)settings.DetailLevel;
            Halftone = settings.Halftone;
            ProjectionLinePatternId = settings.ProjectionLinePatternId.IntegerValue;
            ProjectionLineColor = settings.ProjectionLineColor.GetVizModel();
            ProjectionLineWeight = settings.ProjectionLineWeight;
            IsSurfaceForegroundPatternVisible = settings.IsSurfaceForegroundPatternVisible;
            SurfaceForegroundPatternId = settings.SurfaceForegroundPatternId.IntegerValue;
            SurfaceForegroundPatternColor = settings.SurfaceForegroundPatternColor.GetVizModel();
            IsSurfaceBackgroundPatternVisible = settings.IsSurfaceBackgroundPatternVisible;
            SurfaceBackgroundPatternId = settings.SurfaceBackgroundPatternId.IntegerValue;
            SurfaceBackgroundPatternColor = settings.SurfaceBackgroundPatternColor.GetVizModel();
            SurfaceTransparency = settings.Transparency;
            CutLinePatternId = settings.CutLinePatternId.IntegerValue;
            CutLineColor = settings.CutLineColor.GetVizModel();
            CutLineWeight = settings.CutLineWeight;
            IsCutForegroundPatternVisible = settings.IsCutForegroundPatternVisible;
            CutForegroundPatternId = settings.CutForegroundPatternId.IntegerValue;
            CutForegroundPatternColor = settings.CutForegroundPatternColor.GetVizModel();
            IsCutBackgroundPatternVisible = settings.IsCutBackgroundPatternVisible;
            CutBackgroundPatternId = settings.CutBackgroundPatternId.IntegerValue;
            CutBackgroundPatternColor = settings.CutBackgroundPatternColor.GetVizModel();
        }

        public OverrideGraphicSettings GetOverride()
        {
            OverrideGraphicSettings settings = new OverrideGraphicSettings();

            //settings.SetDetailLevel((ViewDetailLevel)Enum.Parse(typeof(ViewDetailLevel), DetailLevel.ToString()));
            settings.SetHalftone(Halftone);
            settings.SetProjectionLinePatternId(new ElementId(ProjectionLinePatternId));
            if (ProjectionLineColor != null)
            {
                settings.SetProjectionLineColor(ProjectionLineColor.GetColor());
            }
            settings.SetProjectionLineWeight(ProjectionLineWeight);
            settings.SetSurfaceForegroundPatternVisible(IsSurfaceForegroundPatternVisible);
            settings.SetSurfaceForegroundPatternId(new ElementId(SurfaceForegroundPatternId));
            if (SurfaceForegroundPatternColor != null)
            {
                settings.SetSurfaceForegroundPatternColor(SurfaceForegroundPatternColor.GetColor());
            }
            settings.SetSurfaceBackgroundPatternVisible(IsSurfaceBackgroundPatternVisible);
            settings.SetSurfaceBackgroundPatternId(new ElementId(SurfaceBackgroundPatternId));
            if (SurfaceBackgroundPatternColor != null)
            {
                settings.SetSurfaceBackgroundPatternColor(SurfaceBackgroundPatternColor.GetColor());
            }
            settings.SetSurfaceTransparency(SurfaceTransparency);
            settings.SetCutLinePatternId(new ElementId(CutLinePatternId));
            if (CutLineColor != null)
            {
                settings.SetCutLineColor(CutLineColor.GetColor());
            }
            settings.SetCutLineWeight(CutLineWeight);
            settings.SetCutForegroundPatternVisible(IsCutForegroundPatternVisible);
            settings.SetCutForegroundPatternId(new ElementId(CutForegroundPatternId));
            if (CutForegroundPatternColor != null)
            {
                settings.SetCutForegroundPatternColor(CutForegroundPatternColor.GetColor());
            }
            settings.SetCutBackgroundPatternVisible(IsCutBackgroundPatternVisible);
            settings.SetCutBackgroundPatternId(new ElementId(CutBackgroundPatternId));
            if (CutBackgroundPatternColor != null)
            {
                settings.SetCutBackgroundPatternColor(CutBackgroundPatternColor.GetColor());
            }
            return settings;
        }
    }
}
