import { VoidFunctionComponent, useContext } from "react";
import DataGrid, { Column } from "devextreme-react/data-grid";
import { ProductionContext } from "./Context";

const Productions: VoidFunctionComponent = () => {
  const { dataSource } = useContext(ProductionContext);

  return (
    <div>
      <DataGrid keyExpr="id" allowColumnReordering={true} allowColumnResizing={true} columnAutoWidth={true} showBorders={true} dataSource={dataSource}>
        <Column dataField="id" caption="Id" />
        <Column dataField="generatorId" caption="Generator" />
        <Column caption="Date">
          <Column dataField="date" caption="Date" />
          <Column dataField="time" caption="Time" />
        </Column>
        <Column caption="Quantity">
          <Column dataField="quantity" caption="kWh" />
        </Column>
      </DataGrid>
    </div>
  );
};

export default Productions;
