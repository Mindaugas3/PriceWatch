import React, { Component } from 'react';
import Purple from './common/Purple';
import White from './common/White';
import Block from './common/Block';
// import Button from 'bootstrap/Button';
import "./common/PwCss.css";

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <Block height="max-content" isFlex padding="10px">
        <h1>
          Sometimes, <White>data</White> is all you need. <br/> 
          Choose the <White>cheapest</White> product. <br/>
          Pay and wait <White>less</White> for shipping <br/>
        </h1>
        <img src={process.env.PUBLIC_URL + "/hero_img.png"} alt="shopping cart" className="float-right"/>
        <h5 className="break-word">
          Don't waste time searching and comparing prices. Let <White bold>us</White> do the job for you! <br/>
        </h5>
        <div className="h-fit-content">
          <button className="getStartedBtn">Get started (looking glass icon)</button>
        </div>
      </Block>
    );
  }
}
