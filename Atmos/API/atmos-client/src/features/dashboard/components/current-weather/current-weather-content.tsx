import { FlexColumn } from "@/ui/layout/flexbox";
import { CurrentWeatherCard } from "./current-weather-card";

import StaticHumiditySvg from "@/assets/humidity-static.svg";
import HumiditySvg from "@/assets/humidity.svg";
import StaticMistSvg from "@/assets/mist-static.svg";
import MistSvg from "@/assets/mist.svg";
import StaticThermometerSvg from "@/assets/thermometer-static.svg";
import ThermometerSvg from "@/assets/thermometer.svg";

type CurrentWeatherCardProps = {
  temperature?: number;
  humidity?: number;
  dewPoint?: number;
};

export const CurrentWeatherContent = ({
  temperature,
  humidity,
  dewPoint,
}: CurrentWeatherCardProps) => {
  return (
    <FlexColumn width={"100%"} spacing={1} sx={{ marginTop: 2 }}>
      <CurrentWeatherCard
        label="Temperature"
        unit="°F"
        value={temperature ?? 0}
        iconSrc={StaticThermometerSvg}
        animatedIconSrc={ThermometerSvg}
      />
      <CurrentWeatherCard
        label="Humidity"
        unit="%"
        value={humidity ?? 0}
        iconSrc={StaticHumiditySvg}
        animatedIconSrc={HumiditySvg}
      />
      <CurrentWeatherCard
        label="Dew Point"
        unit="°F"
        value={dewPoint ?? 0}
        iconSrc={StaticMistSvg}
        animatedIconSrc={MistSvg}
      />
    </FlexColumn>
  );
};
