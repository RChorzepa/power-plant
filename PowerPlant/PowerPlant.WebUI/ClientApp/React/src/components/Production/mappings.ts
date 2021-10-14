import { IProduction, IProductionMapped, ITime, ICriterias, ICriteriasResponse } from "./interfaces";

export const mapQuery = (query: any): object => {
  let queryObject: any = {};

  for (let key in query) {
    if (query[key] === null) continue;

    if (Array.isArray(query[key]) && query[key].length) {
      if (query[key][0].hasOwnProperty("_isAMomentObject")) {
        queryObject[key] = { min: query[key][0].toDate(), max: query[key][1].toDate() };
      } else {
        queryObject[key] = query[key];
      }
    } else {
      queryObject[key] = query[key];
    }
  }

  return { ...queryObject };
};

export const mapProductionResponse = (data: IProduction[]): IProductionMapped[] => {
  const collection: IProductionMapped[] = data.map((item) => {
    return {
      id: item.id,
      generatorId: item.generatorId,
      quantity: item.quantity,
      date: new Date(item.date),
      time: item.time,
    };
  });

  return collection;
};

export const mapTime = (time: ITime): string => {
  const hours = time.hours <= 9 ? `0${time.hours}` : `${time.hours}`;
  const minutes = time.minutes <= 9 ? `0${time.minutes}` : `${time.minutes}`;
  const seconds = time.seconds <= 9 ? `0${time.seconds}` : `${time.seconds}`;

  return `${hours}:${minutes}:${seconds}:${time.milliseconds}`;
};

export const createTimeWithoutTime = (value: string): Date => {
  const newDate = new Date(value);
  newDate.setHours(0, 0, 0, 0);

  return newDate;
};

export const mapCriterias = (response: ICriteriasResponse): ICriterias => {
  const criterias: ICriterias = {
    generators: response.generators.sort((a, b) => a - b),
    dateRange: {
      min: createTimeWithoutTime(response.dateRange.min),
      max: createTimeWithoutTime(response.dateRange.max),
    },
  };

  return criterias;
};
