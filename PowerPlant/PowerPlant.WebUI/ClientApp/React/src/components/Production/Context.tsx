import { FunctionComponent, createContext, useState, useEffect } from "react";
import axios from "axios";
import { debounce } from "lodash";
import { IPaginationResponse, IProductionContext, IProductionMapped, IPaginationSetup, ICriteriasResponse, ICriterias, IQueryValues } from "./interfaces";
import { mapQuery, mapProductionResponse, mapCriterias } from "./mappings";

export const ProductionContext = createContext<IProductionContext>({
  dataSource: [],
  query: {},
  pagination: {
    current: 1,
    pageSize: 20,
    total: 0,
    showQuickJumper: true,
    showSizeChanger: true,
    showTotal: (total: number) => "",
    onChange: (page: number, pageSize?: number) => {},
  },
  criterias: {} as ICriterias,
  totalItems: 0,
  setQueryValues: (values: IQueryValues) => {},
  pending: false,
});

export const ProductionProvider: FunctionComponent = ({ children }) => {
  const [dataSource, setDataSource] = useState<IProductionMapped[]>([]);
  const [query, setQuery] = useState<IQueryValues>({} as IQueryValues);
  const [pending, setPending] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalItems, setTotalItems] = useState(0);
  const [criterias, setCriterias] = useState<ICriterias>({} as ICriterias);

  useEffect(() => {
    getCriterias();
  }, []);

  useEffect(() => {
    getPagedData(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage, pageSize, query]);

  useEffect(() => {
    // getPagedData(currentPage, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [query]);

  const getPagedData = async (page: number, pageSize: number) => {
    try {
      setPending(true);

      const response = await axios.get("production", {
        params: {
          filter: JSON.stringify(mapQuery(query)),
          page,
          pageSize,
        },
      });

      const data = response.data as IPaginationResponse;

      setCurrentPage(data.currentPage);
      setPageSize(data.pageSize);
      setTotalItems(data.totalItems);
      setDataSource(mapProductionResponse(data.items));
    } finally {
      setPending(false);
    }
  };

  const getCriterias = async () => {
    const response = await axios.get("production/criterias");

    const data = (await response.data) as ICriteriasResponse;
    setCriterias(mapCriterias(data));
  };

  const setQueryValues = debounce((values: IQueryValues) => setQuery(values), 1000);

  const pagination: IPaginationSetup = {
    total: totalItems,
    current: currentPage,
    pageSize: pageSize,
    showQuickJumper: true,
    showSizeChanger: true,
    showTotal: (total: number) => `Total ${total} items`,
    onChange: (page: number, pageSize?: number) => {
      setCurrentPage(page);
      pageSize && setPageSize(pageSize);
    },
  };

  return (
    <ProductionContext.Provider
      value={{
        dataSource,
        query,
        pagination,
        pending,
        criterias,
        totalItems,
        setQueryValues,
      }}
    >
      {children}
    </ProductionContext.Provider>
  );
};
