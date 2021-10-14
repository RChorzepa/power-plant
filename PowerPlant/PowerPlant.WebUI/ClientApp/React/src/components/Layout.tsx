import { FunctionComponent } from "react";
import styled from "styled-components";
import Header from "./Header";

const Wrapper = styled.div`
  position: relative;
`;

const Content = styled.div`
  padding: 20px;
`;

const Layout: FunctionComponent = ({ children }) => (
  <Wrapper>
    <Header title="Power plant" />
    <Content>{children}</Content>
  </Wrapper>
);

export default Layout;
