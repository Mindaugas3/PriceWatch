import React from "react";
import Block from "../common/Block";
import AdminLayout from "./adminLayout";

interface Source { 
    id: String | Number | JSX.Element; 
}

interface SourceControlProps {
    sources: Source[];
}

export default function SourceControl({ sources }: SourceControlProps) {
    return (
        <AdminLayout>
        {
            sources.map((source: Source) => <Block isFlex>{source.id}</Block>)
        }
        </AdminLayout>
    )
}