import ImageList from "./image-list";
import React from "react";
import { useSelector } from "react-redux";
import { StateType } from "../../store";
import { Status } from "../../reducer/status";
import { Loader } from "@stardust-ui/react";
import { useTranslation } from "react-i18next";
import { ConfigPage } from "../../locales";

const List: React.FC = () => {
    const status = useSelector((state: StateType) => state.status);
    const stickes = useSelector((state: StateType) => state.stickers);
    const { t } = useTranslation();
    return status === Status.pending ? (
        <Loader styles={{ padding: "2em 0" }} label={t(ConfigPage.loading)} size="larger" />
    ) : (
        <ImageList items={stickes} />
    );
};

export default List;
