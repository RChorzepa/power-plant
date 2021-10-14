import { VoidFunctionComponent } from "react";
import styled from "styled-components";
import axios from "axios";
import { Modal, Row, Col, InputNumber, Button, Form, notification } from "antd";

const MessageWrapper = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  span {
    text-align: center;
  }
`;

interface IFormValues {
  year: number;
  generators: number;
}

const Generate: VoidFunctionComponent<{ visible: boolean; setVisible: (value: boolean) => void }> = ({ visible, setVisible }) => {
  const onFinish = async (values: IFormValues) => {
    try {
      await axios.get("production/generate", {
        params: { ...values },
      });
      notification.success({
        message: "Success",
        description: `Generating task fake data was added to the queue.`,
      });
    } catch (error) {
      notification.error({
        message: "Failure",
        description: "An error has occurred  while generating fake data.",
      });
    } finally {
      setVisible(false);
    }
  };

  return (
    <Modal visible={visible} title="Generating fake data by year and generators quantity." onCancel={() => setVisible(false)} okButtonProps={{ hidden: true }}>
      <Row>
        <Col span={24}>
          <MessageWrapper>
            <span>Please select year and generators quantity.</span>
            <span>Generating fake data could take a while.</span>
          </MessageWrapper>
        </Col>
      </Row>
      <Row>
        <Col span={24}>
          <Form layout="vertical" onFinish={onFinish} initialValues={{ year: 2019, generators: 1 }}>
            <Form.Item label="Year" name="year" rules={[{ required: true }]}>
              <InputNumber style={{ width: "100%" }} />
            </Form.Item>
            <Form.Item label="Generators quantity" name="generators" rules={[{ required: true }]}>
              <InputNumber style={{ width: "100%" }} min={1} max={20} />
            </Form.Item>
            <Form.Item>
              <Button type="primary" htmlType="submit">
                Generate
              </Button>
            </Form.Item>
          </Form>
        </Col>
      </Row>
    </Modal>
  );
};

export default Generate;
