"use client";

// import { observer } from "mobx-react-lite";
import { Table, Button, Tooltip } from "antd";
import Link from "next/link";
import { images } from "@/theme";
import Image from "next/image";
import { PaperClipOutlined, UploadOutlined } from "@ant-design/icons";
import { Manrope } from "next/font/google";
const manrope = Manrope({ subsets: ["latin"] });

const Page = () => {
  const dataSource = [
    {
      key: 1,
      "post date": "25/04/2022",
      "created by": "PINNEK, EMESHIE LTD -  PINN-8081",
    },
  ];

  return (
    <div className={manrope.className}>
      <div className="m-3 w-4/5 mx-auto">
        <p className="flex justify-start gap-2 items-center text-base px-5 w-full capitalize bg-[#194bfb] text-white rounded-t-lg h-10">
          <span>
            <Image
              src={images.pin_icon}
              alt="icon"
              className="cursor-pointer  text-[10px] "
            />
          </span>
          CSC
        </p>
        <Table
          className="drop-shadow-lg"
          rowKey="key"
          bordered
          columns={[
            {
              title: <p className="text-base">Upload Date</p>,
              dataIndex: "post date",
              key: "post date",
              align: "center",
            },
            {
              title: <p className="text-base">Created By</p>,
              dataIndex: "created by",
              key: "created by",
              align: "center",
              render: (text) => (
                <p className="font-semibold text-sm leading-5" >{text}</p>
              )
            },
            {
              title: <p className="text-base">Actions</p>,
              key: "actions",
              render: (text, record) => (
                <div className="flex justify-center">
                  <Tooltip title="Upload">
                    <Link href='/user/capital-market/portfolio-management/Send-Customer-Stock-Position-To-Email-Yse-FoxPro-Upload/upload' className="p-0 border-0">
                    <UploadOutlined style={{fontSize:'25px',color:'black'}}/>
                    </Link>
                  </Tooltip>
                </div>
              ),
              align: "center",
            },
          ]}
          dataSource={dataSource}
          scroll={{ x: 728 }}
          size="small"
        ></Table>
      </div>
    </div>
  );
};

export default Page;
