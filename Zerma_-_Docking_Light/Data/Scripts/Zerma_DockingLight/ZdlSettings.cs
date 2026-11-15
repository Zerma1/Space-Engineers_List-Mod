using System;
using System.Globalization;
using System.Text;

namespace Zerma.DockingLight
{
    // Bornes et constantes propres à chaque taille de grille (SPEC_V2 §2, §4).
    public class ZdlLimits
    {
        public float DistanceMin, DistanceMax, DistanceDefault;
        public float SpacingMin, SpacingMax, SpacingDefault;
        public float SizeMin, SizeMax, SizeDefault;
        public float OffsetMax;
        public int MaxGlobes;
        public float PowerBaseMW, PowerPerGlobeMW;
        public float IntensityMin, IntensityMax, IntensityDefault;
        public double CameraMaxDistance;
        public float LightZFallback;
        public float StepScale;                      // pas des actions : grande grille = table, petite grille = moitié

        public static readonly ZdlLimits Large = new ZdlLimits
        {
            DistanceMin = 5f, DistanceMax = 100f, DistanceDefault = 20f,
            SpacingMin = 1f, SpacingMax = 10f, SpacingDefault = 2f,
            SizeMin = 0.2f, SizeMax = 1f, SizeDefault = 0.5f,
            OffsetMax = 10f,
            MaxGlobes = 100,
            PowerBaseMW = 0.0005f, PowerPerGlobeMW = 0.0002f,
            IntensityMin = 0.5f, IntensityMax = 5f, IntensityDefault = 1f,
            CameraMaxDistance = 2000,
            LightZFallback = -1.195f,
            StepScale = 1f,
        };

        public static readonly ZdlLimits Small = new ZdlLimits
        {
            DistanceMin = 2f, DistanceMax = 50f, DistanceDefault = 10f,
            SpacingMin = 0.5f, SpacingMax = 5f, SpacingDefault = 1f,
            SizeMin = 0.1f, SizeMax = 0.5f, SizeDefault = 0.2f,
            OffsetMax = 5f,
            MaxGlobes = 50,
            PowerBaseMW = 0.00025f, PowerPerGlobeMW = 0.0001f,
            IntensityMin = 0.5f, IntensityMax = 5f, IntensityDefault = 3f,
            CameraMaxDistance = 1000,
            LightZFallback = -0.217f,
            StepScale = 0.5f,
        };

        public const float PeriodMin = 0.2f, PeriodMax = 10f, PeriodDefault = 1.5f;
        public const float DutyMin = 5f, DutyMax = 95f, DutyDefault = 50f;
        public const float MinLevelMax = 50f;
        // Quantification des décalages : 0,005 m, diviseur de tous les pas d'action (petite grille : 0,025 / 0,125 / 0,25 / 0,5 / 1,25 m).
        public const float OffsetStep = 0.005f;

        // Pas sélectionnables pour les actions +/- (valeurs grande grille ; petite grille = × StepScale).
        public static readonly float[] DistanceSteps = { 1f, 5f, 10f, 25f };
        public static readonly float[] SpacingSteps = { 0.1f, 0.5f, 1f, 2f };
        public static readonly float[] OffsetSteps = { 0.05f, 0.25f, 0.5f, 1f, 2.5f };
        public const long DistanceStepDefault = 1, SpacingStepDefault = 1, OffsetStepDefault = 1;

        public float Step(float[] table, long index)
        {
            return table[index] * StepScale;
        }
    }

    public enum ZdlBlinkMode : long
    {
        None = 0,
        Global = 1,
        WaveOut = 2,
        WaveIn = 3,
    }

    public class ZdlSettings
    {
        public float Distance, Spacing, GlobeSize;
        public long BlinkMode;
        public float BlinkPeriod = ZdlLimits.PeriodDefault, BlinkDuty = ZdlLimits.DutyDefault, MinLevel = 0f;
        public float OffsetV, OffsetH;
        public long DistanceStepIndex = ZdlLimits.DistanceStepDefault;
        public long SpacingStepIndex = ZdlLimits.SpacingStepDefault;
        public long OffsetVStepIndex = ZdlLimits.OffsetStepDefault;
        public long OffsetHStepIndex = ZdlLimits.OffsetStepDefault;

        public static ZdlSettings CreateDefault(ZdlLimits limits)
        {
            var s = new ZdlSettings();
            s.Distance = limits.DistanceDefault;
            s.Spacing = limits.SpacingDefault;
            s.GlobeSize = limits.SizeDefault;
            return s;
        }

        public ZdlSettings Clone()
        {
            return (ZdlSettings)MemberwiseClone();
        }

        public void Clamp(ZdlLimits limits)
        {
            Distance = Bound(Distance, limits.DistanceMin, limits.DistanceMax, limits.DistanceDefault);
            Spacing = Bound(Spacing, limits.SpacingMin, limits.SpacingMax, limits.SpacingDefault);
            GlobeSize = Bound(GlobeSize, limits.SizeMin, limits.SizeMax, limits.SizeDefault);
            if (BlinkMode < 0 || BlinkMode > 3) BlinkMode = 0;
            BlinkPeriod = Bound(BlinkPeriod, ZdlLimits.PeriodMin, ZdlLimits.PeriodMax, ZdlLimits.PeriodDefault);
            BlinkDuty = Bound(BlinkDuty, ZdlLimits.DutyMin, ZdlLimits.DutyMax, ZdlLimits.DutyDefault);
            MinLevel = Bound(MinLevel, 0f, ZdlLimits.MinLevelMax, 0f);
            OffsetV = RoundToStep(Bound(OffsetV, -limits.OffsetMax, limits.OffsetMax, 0f));
            OffsetH = RoundToStep(Bound(OffsetH, -limits.OffsetMax, limits.OffsetMax, 0f));
            DistanceStepIndex = BoundIndex(DistanceStepIndex, ZdlLimits.DistanceSteps.Length, ZdlLimits.DistanceStepDefault);
            SpacingStepIndex = BoundIndex(SpacingStepIndex, ZdlLimits.SpacingSteps.Length, ZdlLimits.SpacingStepDefault);
            OffsetVStepIndex = BoundIndex(OffsetVStepIndex, ZdlLimits.OffsetSteps.Length, ZdlLimits.OffsetStepDefault);
            OffsetHStepIndex = BoundIndex(OffsetHStepIndex, ZdlLimits.OffsetSteps.Length, ZdlLimits.OffsetStepDefault);
        }

        // Index de pas hors table → pas par défaut.
        private static long BoundIndex(long index, int count, long fallback)
        {
            return index >= 0 && index < count ? index : fallback;
        }

        private static float Bound(float value, float min, float max, float fallback)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return fallback;
            return Math.Max(min, Math.Min(max, value));
        }

        private static float RoundToStep(float value)
        {
            return (float)(Math.Round(value / ZdlLimits.OffsetStep) * ZdlLimits.OffsetStep);
        }

        // Format versionné : "v1;distance;espacement;taille;mode;periode;duty;min;halo[;offsetV;offsetH[;pasDist;pasEsp;pasOffV;pasOffH]]"
        // (champs optionnels absents → valeurs par défaut)
        // Le champ halo n'est plus utilisé (halo toujours actif) : écrit à 1 et ignoré à la lecture, pour garder le format stable.
        public string Serialize()
        {
            var c = CultureInfo.InvariantCulture;
            var sb = new StringBuilder("v1");
            sb.Append(';').Append(Distance.ToString("R", c));
            sb.Append(';').Append(Spacing.ToString("R", c));
            sb.Append(';').Append(GlobeSize.ToString("R", c));
            sb.Append(';').Append(BlinkMode.ToString(c));
            sb.Append(';').Append(BlinkPeriod.ToString("R", c));
            sb.Append(';').Append(BlinkDuty.ToString("R", c));
            sb.Append(';').Append(MinLevel.ToString("R", c));
            sb.Append(';').Append('1');
            sb.Append(';').Append(OffsetV.ToString("R", c));
            sb.Append(';').Append(OffsetH.ToString("R", c));
            sb.Append(';').Append(DistanceStepIndex.ToString(c));
            sb.Append(';').Append(SpacingStepIndex.ToString(c));
            sb.Append(';').Append(OffsetVStepIndex.ToString(c));
            sb.Append(';').Append(OffsetHStepIndex.ToString(c));
            return sb.ToString();
        }

        // Une valeur illisible garde la valeur par défaut (et non le minimum après bornage).
        public static ZdlSettings Deserialize(string text, ZdlLimits limits)
        {
            var s = CreateDefault(limits);
            if (string.IsNullOrEmpty(text)) return s;
            string[] parts = text.Split(';');
            if (parts.Length < 9 || parts[0] != "v1") return s;
            s.Distance = ParseFloat(parts[1], s.Distance);
            s.Spacing = ParseFloat(parts[2], s.Spacing);
            s.GlobeSize = ParseFloat(parts[3], s.GlobeSize);
            s.BlinkMode = ParseLong(parts[4], s.BlinkMode);
            s.BlinkPeriod = ParseFloat(parts[5], s.BlinkPeriod);
            s.BlinkDuty = ParseFloat(parts[6], s.BlinkDuty);
            s.MinLevel = ParseFloat(parts[7], s.MinLevel);
            if (parts.Length >= 11)
            {
                s.OffsetV = ParseFloat(parts[9], s.OffsetV);
                s.OffsetH = ParseFloat(parts[10], s.OffsetH);
            }
            if (parts.Length >= 15)
            {
                s.DistanceStepIndex = ParseLong(parts[11], s.DistanceStepIndex);
                s.SpacingStepIndex = ParseLong(parts[12], s.SpacingStepIndex);
                s.OffsetVStepIndex = ParseLong(parts[13], s.OffsetVStepIndex);
                s.OffsetHStepIndex = ParseLong(parts[14], s.OffsetHStepIndex);
            }
            s.Clamp(limits);
            return s;
        }

        private static long ParseLong(string text, long fallback)
        {
            long value;
            return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : fallback;
        }

        private static float ParseFloat(string text, float fallback)
        {
            float value;
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ? value : fallback;
        }
    }
}
