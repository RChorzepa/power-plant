import { VoidFunctionComponent } from "react";
import styled from "styled-components";
import { colors } from "../utils/variables";

const Wrapper = styled.div`
  position: relative;
  height: 50px;
  width: 100%auto;
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: center;
  background-color: ${colors.lightGray};
  padding: 0 15px;
`;

const Brand = styled.h4`
  font-size: 15px;
  color: ${colors.primary};
`;

const Header: VoidFunctionComponent<{ title: string }> = ({ title }) => (
  <Wrapper>
    <Brand>{title}</Brand>
  </Wrapper>
);

export default Header;
