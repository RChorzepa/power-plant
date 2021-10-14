import { FunctionComponent, createContext, useState, useEffect } from "react";

export interface IProduction {
  Id: number;
  GeneratorId: number;
  Date: Date;
  Time: Date;
  Quantity: number;
}

interface IProductionContext {
  dataSource: IProduction[];
  getDataSource: () => void;
}

export const ProductionContext = createContext<IProductionContext>({
  dataSource: [],
  getDataSource: () => {},
});

export const ProductionProvider: FunctionComponent = ({ children }) => {
  const [dataSource, setDataSource] = useState<IProduction[]>([]);

  const getDataSource = () => {};

  useEffect(() => {
    fetch("production")
      .then((response) => response.json())
      .then((json) => console.log(json));
  });

  return (
    <ProductionContext.Provider
      value={{
        dataSource,
        getDataSource,
      }}
    >
      {children}
    </ProductionContext.Provider>
  );
};
