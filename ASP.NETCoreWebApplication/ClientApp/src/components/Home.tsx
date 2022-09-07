import React, { Component } from 'react';
import Purple from './common/Purple';
import White from './common/White';
import Block from './common/Block';
// import Button from 'bootstrap/Button';
// import "./common/PwCss.css";
import { Layout } from './Layout';
import Next from "./common/Next";
import Row from "./common/Row";
import ColAuto from "./common/ColAuto";
import { navigateHousingPage} from "../utils/RoutePaths";

const getStarted = () => window.location.href = navigateHousingPage;

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <Layout>
        <Block height="79vh" isFlex padding="10px">
          <Row fullWidth>
            <ColAuto pushOthersToRight>
              <h1>
                Sometimes, <White>data</White> is all you need. <br/> 
                Choose the <White>cheapest</White> product. <br/>
                Pay and wait <White>less</White> for shipping <br/>
              </h1>
            </ColAuto>
            <ColAuto>
              <img src={process.env.PUBLIC_URL + "/hero_img.png"} alt="shopping cart" className="float-right"/>
            </ColAuto>
          </Row>

            <h5 className="break-word w-100 text-center">
              Don't waste time searching and comparing prices. Let <White bold>us</White> do the job for you! <br/>
            </h5>
              <div className="h-fit-content m-auto">
                <button className="getStartedBtn btn-xxl" onClick={getStarted}>Get started{'  '}<Next /></button>
              </div>

        </Block>
      </Layout>
    );
  }
}
