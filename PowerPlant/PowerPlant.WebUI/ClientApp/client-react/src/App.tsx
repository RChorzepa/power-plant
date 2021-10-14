import { VoidFunctionComponent } from "react";
import { Reset } from "styled-reset";
import GlobalStyle from "./utils/GlobalStyle";
import "devextreme/dist/css/dx.light.css";
import Layout from "./components/Layout";
import Production from "./components/Production/Production";
import { ProductionProvider } from "./components/Production/Context";

const App: VoidFunctionComponent = () => (
  <>
    <Reset />
    <GlobalStyle />
    <Layout>
      <ProductionProvider>
        <Production />
      </ProductionProvider>
    </Layout>
  </>
);

export default App;
