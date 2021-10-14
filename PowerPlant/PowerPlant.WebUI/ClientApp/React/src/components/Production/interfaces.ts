export interface ITime {
  days: number;
  hours: number;
  milliseconds: number;
  minutes: number;
  seconds: number;
  ticks: number;
  totalDays: number;
  totalHours: number;
  totalMilliseconds: number;
  totalMinutes: number;
  totalSeconds: number;
}

export interface IProduction {
  id: number;
  generatorId: number;
  date: string;
  quantity: number;
  time: ITime;
}

export interface IPaginationResponse {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  items: IProduction[];
}

export interface IProductionMapped {
  id: number;
  generatorId: number;
  date: Date;
  quantity: number;
  time: ITime;
}

export interface IProductionContext {
  dataSource: IProductionMapped[];
  query: object;
  pagination: IPaginationSetup;
  criterias: ICriterias;
  totalItems: number;
  pending: boolean;
  setQueryValues: (values: IQueryValues) => void;
}

export interface IPaginationSetup {
  total: number;
  current: number;
  pageSize: number;
  showSizeChanger: boolean;
  showQuickJumper: boolean;
  showTotal: (total: number) => string;
  onChange: (page: number, pageSize?: number) => void;
}

export interface ICriteriasResponse {
  dateRange: {
    min: string;
    max: string;
  };
  generators: number[];
}

export interface ICriterias {
  dateRange: {
    min: Date;
    max: Date;
  };
  generators: number[];
}

export interface IQueryValues {
  generator: number[];
  date: {
    min: moment.Moment | null;
    max: moment.Moment | null;
  };
}
