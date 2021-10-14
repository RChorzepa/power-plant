import { VoidFunctionComponent, useContext, useState } from "react";
import { Table, Pagination, Row, Col, Tag, Spin, Card, Button } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import styled from "styled-components";
import { ProductionContext } from "./Context";
import { ITime } from "./interfaces";
import { mapTime } from "./mappings";
import SearchPanel from "./SearchPanel";
import Generate from "./Generate";

const columns = [
  {
    title: "Id",
    dataIndex: "id",
  },
  {
    title: "Generator",
    dataIndex: "generatorId",
  },
  {
    title: "Date",
    dataIndex: "date",
    render: (value: Date) => <span>{value.toLocaleDateString()}</span>,
  },
  {
    title: "Time",
    dataIndex: "time",
    render: (value: ITime) => <span>{mapTime(value)}</span>,
  },
  {
    title: "Quantity",
    dataIndex: "quantity",
    render: (value: number) => (
      <span>
        {value.toFixed(6)} <Tag color="blue">kWh</Tag>
      </span>
    ),
  },
];

const PaginationWrapper = styled.div`
  margin-top: 20px;
  display: flex;
  justify-content: end;
`;

const ButtonWrapper = styled.div`
  margin-top: 20px;
`;

const Productions: VoidFunctionComponent = () => {
  const { dataSource, pagination, pending, totalItems } = useContext(ProductionContext);
  const [modaVisible, setModalVisible] = useState(false);

  return (
    <>
      <Row>
        <Col span={24}>
          <SearchPanel />
        </Col>
      </Row>
      <Row style={{ marginTop: "15px" }}>
        <Col span={24}>
          <Card
            title={
              <Row justify="space-between">
                <Col>Generators production data</Col>
                <Col>
                  <Tag color="magenta">
                    Total items:<strong> {totalItems}</strong>
                  </Tag>
                </Col>
              </Row>
            }
          >
            <Spin spinning={pending} tip="Please wait..." indicator={<LoadingOutlined style={{ fontSize: 24 }} spin />}>
              <Table rowKey="id" size="small" columns={columns} dataSource={dataSource} pagination={false} />
            </Spin>
          </Card>
        </Col>
      </Row>
      <Row>
        <Col span={12}>
          <ButtonWrapper>
            <Button type="primary" onClick={() => setModalVisible(true)}>
              Generate fake data
            </Button>
          </ButtonWrapper>
        </Col>

        <Col span={12}>
          <PaginationWrapper>
            <Pagination {...pagination} />
          </PaginationWrapper>
        </Col>
      </Row>
      <Generate visible={modaVisible} setVisible={(value: boolean) => setModalVisible(value)} />
    </>
  );
};

export default Productions;
