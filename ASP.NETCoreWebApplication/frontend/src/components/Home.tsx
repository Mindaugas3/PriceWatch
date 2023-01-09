import React, { Component } from "react";
import { navigateHousingPage } from "../utils/route-paths";
import { NextArrow } from "./common/next-arrow";
import { Layout } from "./Layout";

const getStarted = () => (window.location.href = navigateHousingPage);

export class Home extends Component {
    render() {
        return (
            <Layout>
                <div className="container">
                    <div className="row">
                        <div className="col-7">
                            <h1>
                                Sometimes, data is all you need. <br />
                                Choose the cheapest product. <br />
                                Pay and wait less for shipping <br />
                            </h1>
                        </div>
                        <div className="col-5">
                            <img
                                src={process.env.PUBLIC_URL + "/hero_img.png"}
                                alt="shopping cart"
                                className="img-fluid"
                            />
                        </div>
                        <div className="row">
                            <h5 className="break-word w-100 text-center">
                                Don't waste time searching and comparing prices. Let us do the job for you! <br />
                            </h5>
                        </div>
                        <div className="row">
                            <button className="getStartedBtn btn-xxl" onClick={getStarted}>
                                Get started{"  "}
                                <NextArrow />
                            </button>
                        </div>
                    </div>
                </div>
            </Layout>
        );
    }
}
