import React from "react";
import AdminNavbar from "./adminNavbar";
import Container from "reactstrap/lib/Container";

type ArbitraryOutputValue = JSX.Element | JSX.Element[] | String | Number;

interface AdminLayoutProps {
  children: ArbitraryOutputValue;
}

export default function AdminLayout ({ children }: AdminLayoutProps){
    return (
    <div>
    <AdminNavbar />
    <Container>
      {children}
    </Container>
  </div>
    )
}