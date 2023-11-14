"use client";

import { useStores } from "@/hooks/use-store";
import { SearchOutlined } from "@ant-design/icons";
import { Button, Input, Select, Spin, Table, Tooltip } from "antd";
import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import type { ColumnsType } from "antd/es/table";
import Link from "next/link";
import { images } from "@/theme";
import Image from "next/image";
import { useRouter } from "next/navigation";

export default observer(function Page() {
  const { depositStore } = useStores();

  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [isloading, setIsloading] = useState(true);
  const [isOpen, setIsOpen] = useState(false);
  const [selfBalancingStatus, setselfBalancingStatus] = useState("All");
  const [tableData, setTableData] = useState([]);

  const router = useRouter();

  const navigateToDetailsPage = (code: String) => {
    router.push(`/user/accounting/customer-deposit/${code}`);
  };
  interface DataType {
    key: React.Key;
    TxnID: string;
    account: number;
    amount: number;
    status: string;
    createdBY: string;
    date: string;
    dataSource: [];
  }

  const columns: ColumnsType<DataType> = [
    {
      title: "Date",
      responsive: ["sm"],
      // width: 80,
      dataIndex: "vDate",
      key: "vDate",
      render: (text: String) => <p>{text.slice(0, 10)}</p>,
    },
    {
      title: "Debit Account",
      responsive: ["sm"],
      // width: 130,
      dataIndex: "mainAcct",
      key: "mainAcct",
      sorter: true,
      render: (text) => (
        <div className="flex flex-col capitalize">
          <p className="font-[600]">{text.accountId.toLowerCase()}</p>
          <p className="text-[12px]">{text.accountName.toLowerCase()}</p>
        </div>
      ),
    },
    {
      title: "Amount",
      responsive: ["sm"],
      // width: 80,
      dataIndex: "amount",
      key: "amount",
      sorter: (a: any, b: any) => a.amount - b.amount,
      render: (text) => <p className="font-[600]">{text}</p>,
    },
    {
      title: "Credit Account",
      responsive: ["sm"],
      // width: 130,
      dataIndex: "conAcct",
      key: "conAcct",
      sorter: true,
      render: (text) => (
        <div className="flex flex-col capitalize">
          <p className="font-[600]">{text.accountId.toLowerCase()}</p>
          <p className="text-[12px]">{text.accountName.toLowerCase()}</p>
        </div>
      ),
    },
    {
      title: "Txn ID",
      responsive: ["sm"],
      // width: 80,
      dataIndex: "transNo",
      key: "transNo",
      sorter: (a: any, b: any) => a.transNo.length - b.transNo.length,
    },

    {
      title: "Status",
      responsive: ["sm"],
      // width: 80,
      dataIndex: "status",
      key: "status",
      render: (text) => (
        <p
          className={
            text === "UnPosted"
              ? "border border-[#FF784B] text-center  w-full font-bold text-[11px] rounded-lg text-[#FF784B] bg-[#FEE2E2]"
              : text === "Reversed"
              ? "border border-[#FACC15] text-center  w-full font-bold text-[11px] rounded-lg text-[#FACC15] bg-[#FFFCF0]"
              : text === "Posted"
              ? "border border-[#22C55E] text-center  w-full font-bold text-[11px] rounded-lg text-[#22C55E] bg-[#F6FDF9]"
              : ""
          }
        >
          {text === "Posted"
            ? "Approved"
            : text === "Reversed"
            ? "Reversed"
            : "Unposted"}
        </p>
      ),
    },

    {
      title: "Actions",
      key: "action",
      responsive: ["sm"],
      width: 140,
      render: (record) => (
        <div className="flex justify-evenly items-center gap-2">
          <Tooltip placement="bottom" title="approve">
            <Image
              src={images.checkIncon}
              alt="icon"
              className="cursor-pointer w-[15%] text-[10px] text-white"
              onClick={() => navigateToDetailsPage(record.code)}
            />
          </Tooltip>
          <Tooltip placement="bottom" title="reverse">
            <Image
              src={images.refreshIcon}
              alt="icon"
              className="cursor-pointer w-[15%] text-[10px] "
              onClick={() => navigateToDetailsPage(record.code)}
            />
          </Tooltip>
          <Tooltip placement="bottom" title="edit">
            <Image
              src={images.pencil_icon}
              alt="icon"
              className="cursor-pointer w-[15%] text-[10px] "
              onClick={() => navigateToDetailsPage(record.code)}
            />
          </Tooltip>
          <Tooltip placement="bottom" title="delete">
            <Image
              src={images.deleteIcon}
              alt="icon"
              className="cursor-pointer w-[15%] text-[10px] "
              onClick={() => navigateToDetailsPage(record.code)}
            />
          </Tooltip>
        </div>
      ),
    },
  ];
  //function for the filter apply
  const onApplyClick = () => {
    setIsloading(!isloading);
    setIsOpen(false);
  };
  const onSelectChange = (newSelectedRowKeys: []) => {
    console.log("selectedRowKeys changed: ", newSelectedRowKeys);
    setSelectedRowKeys(newSelectedRowKeys);
  };

  const rowSelection = {
    selectedRowKeys,
    onChange: (selectedRowKeys: React.Key[], selectedRows: any[]) => {
      console.log(
        `selectedRowKeys: ${selectedRowKeys}`,
        "selectedRows: ",
        selectedRows
      );

      setSelectedRowKeys(selectedRowKeys);
    },
  };

  const hasSelected = selectedRowKeys.length > 0;

  const fetch = async () => {
    setIsloading(true);
    const fetchTableData = await depositStore.getAllSelfBalancing(
      selfBalancingStatus
    );
    if (fetchTableData.kind === "ok") {
      const mappedData = fetchTableData.data.result.map(
        (value: Object, index: number) => ({
          ...value,
          key: index.toString(),
        })
      );
      setTableData(mappedData);
      setIsloading(false);
    } else {
      setIsloading(false);
      setTableData([]);
    }
  };

  useEffect(() => {
    fetch();
  }, [selfBalancingStatus]);
  return (
    <Spin tip="fetching...." spinning={isloading} className="text-[red] ">
      <main className=" min-h-screen flex flex-col gap-1 w-[95%]  mx-auto ">
        <div className="flex justify-between items-center mb-3">
          <Input
            className="w-[200px]"
            placeholder="Search by Name, TXN ID."
            prefix={<SearchOutlined />}
          />
          <Link href="/user/accounting/journal-posting/self-balancing/New-Self-Balancing">
            <button
              type="button"
              className="inline-flex text-white capitalize bg-[#194BFB] border-0 py-2 px-6 focus:outline-none hover:bg-[#191dfb] rounded text-md"
            >
              self balancing
            </button>
          </Link>
        </div>

        <div className="flex justify-between flex-wrap ">
          <div className="flex flex-col text-[#718096] justify-start items-start gap-2 ">
            <p className="font-[700] text-[14px] leading-[21px]">Created By</p>
            <Select
              placeholder="kelvin olawale"
              className=" text-[#718096]"
              style={{
                width: 200,
              }}
              options={[
                {
                  value: "kelvin olawale",
                  label: "kelvin olawale",
                },
              ]}
            />
          </div>

          <div className="flex flex-col text-[#718096] justify-start items-start gap-2">
            <p className="font-[700] text-[14px] leading-[21px]">
              Product Account
            </p>
            <Select
              placeholder="Account"
              style={{
                width: 200,
              }}
              className=" text-[#718096]"
              options={[
                {
                  value: "2",
                  label: "Account",
                },
                {
                  value: "3",
                  label: "Account",
                },
              ]}
            />
          </div>

          <div className="flex flex-col text-[#718096] justify-start items-start gap-2">
            <p className="font-[700] text-[14px] leading-[21px]">Date</p>
            <Select
              placeholder="10th-feb - 10th-sept"
              className=" text-[#718096]"
              style={{
                width: 200,
              }}
              options={[
                {
                  value: "10th-feb - 10th-sept",
                  label: "10th-feb - 10th-sept",
                },
              ]}
            />
          </div>
          <div className="flex flex-col text-[#718096] justify-start items-start gap-2">
            <p className="font-[700] text-[14px] leading-[21px]">Amount</p>
            <Select
              placeholder="1000-100,000"
              className=" text-[#718096]"
              style={{
                width: 200,
              }}
              options={[
                {
                  value: "1000-100,000",
                  label: "1000-100,000",
                },
              ]}
            />
          </div>

          <div className="flex flex-col text-[#718096] justify-start items-start gap-2">
            <p className="font-[700] text-[14px] leading-[21px]">status</p>

            <Select
              placeholder="Select Status"
              style={{
                width: 200,
              }}
              className="text-[#718096]"
              defaultValue="All"
              onSelect={setselfBalancingStatus}
              options={[
                {
                  value: "Posted",
                  label: "Approved",
                },
                {
                  value: "UnPosted",
                  label: "Pending",
                },
                {
                  value: "Reversed",
                  label: "Reversed",
                },
                {
                  value: "All",
                  label: "All",
                },
              ]}
            />
          </div>
        </div>
        <div>
          {hasSelected && (
            <Button className="inline-flex text-white capitalize bg-[#194BFB] border-0 focus:outline-none hover:bg-[#191dfb] rounded text-md text-center">
              Reload Table
            </Button>
          )}
        </div>

        <div className=" w-full ">
          <Table
            sticky={{
              offsetHeader: 50,
            }}
            scroll={{ x: "max-content" }}
            rowSelection={rowSelection}
            columns={columns}
            dataSource={tableData}
          />
        </div>
      </main>
    </Spin>
  );
});
