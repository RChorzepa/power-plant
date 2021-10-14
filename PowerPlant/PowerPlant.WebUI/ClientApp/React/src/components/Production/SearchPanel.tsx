import { VoidFunctionComponent, useContext, useState, useEffect } from "react";
import moment from "moment";
import { isEmpty } from "lodash";
import { ProductionContext } from "./Context";
import { DatePicker, Card, Row, Col, Typography, Select } from "antd";
import { IQueryValues } from "./interfaces";

const Text = Typography.Text;
const { Option } = Select;

const dateFormat = "YYYY-MM-DD";

const SearchPanel: VoidFunctionComponent = () => {
  const { criterias, setQueryValues } = useContext(ProductionContext);
  const [state, setState] = useState<IQueryValues>({} as IQueryValues);

  useEffect(() => {
    !isEmpty(criterias) && setQueryValues(state);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state]);

  const onDateRangeChange = (values: moment.Moment[]) => {
    setState((prev) => ({
      ...prev,
      date:
        values != null
          ? {
              min: values[0],
              max: values[1],
            }
          : {
              min: null,
              max: null,
            },
    }));
  };

  const onGeneratorChange = (values: number[]) => {
    setState((prev) => ({
      ...prev,
      generator: values.length ? values : [],
    }));
  };

  return (
    <Card>
      <Row gutter={20} align="bottom">
        <Col span={6}>
          <Row>
            <Col span={24}>
              <Text>Date range: </Text>
              {criterias.dateRange ? (
                <DatePicker.RangePicker
                  format={dateFormat}
                  style={{ width: "100%" }}
                  defaultValue={[moment(criterias.dateRange?.min, dateFormat), moment(criterias.dateRange?.max, dateFormat)]}
                  onChange={(e) => onDateRangeChange(e as moment.Moment[])}
                />
              ) : (
                <DatePicker.RangePicker />
              )}
            </Col>
          </Row>
        </Col>
        <Col span={6}>
          <Row>
            <Col span={24}>
              <Text>Generators: </Text>
              <Select mode="multiple" placeholder="Please select generators" style={{ width: "100%" }} onChange={onGeneratorChange}>
                {criterias.generators &&
                  criterias.generators.map((item) => (
                    <Option key={item} value={item}>
                      {item}
                    </Option>
                  ))}
              </Select>
            </Col>
          </Row>
        </Col>
      </Row>
    </Card>
  );
};

export default SearchPanel;
