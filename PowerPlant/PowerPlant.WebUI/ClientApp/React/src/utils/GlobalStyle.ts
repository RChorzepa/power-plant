import { createGlobalStyle } from "styled-components";
import { global, colors } from "./variables";

const GlobalStyle = createGlobalStyle`
 * {
        box-sizing: border-box;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
    }

    body {
        font-family: ${global.fontFamily};
        font-size: ${global.fontSize};
        font-weight: ${global.fontWeight};
        color: ${colors.defaultFontColor};
        background-color: ${global.backgroundColor};
        &::-webkit-scrollbar-track
        {
            box-shadow: inset 0 0 10px rgba(0,0,0,0.3);
            border-radius: 2px;
            background-color: #FFF;
        }
        &::-webkit-scrollbar
        {
            width: 12px;
            background-color: #F5F5F5;
        }
        &::-webkit-scrollbar-thumb
        {
            border-radius: 2px;
            box-shadow: inset 0 0 10px rgba(0,0,0,.3);
            background-color: ${colors.silver};
        }
    }
`;

export default GlobalStyle;
