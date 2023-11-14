"use client";

import { useStores } from "@/hooks/use-store";
import { SearchOutlined } from "@ant-design/icons";
import { Button, Input, Select, Spin, Table } from "antd";
import type { ColumnsType } from "antd/es/table";
import { observer } from "mobx-react-lite";
import Link from "next/link";
import { useEffect, useState } from "react";

interface DataType {
  key: React.Key;
  TxnID: string;
  account: {
    accountName?: string;
    AccountId: string;
  };
  amount: number;
  status: string;
  createdBY: string;
  date: string;
  dataSource: [];
  batchSpreadSheetMasterId: number;
  record: {
    batchSpreadSheetMasterId: number;
    account: {
      accountName: string;
      AccountId: string;
    };
  };
}

export default observer(function Page() {
  const { depositStore } = useStores();

  const [isloading, setIsloading] = useState(true);
  const [tableData, setTableData] = useState([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);

  const columns: ColumnsType<DataType> = [
    {
      title: "Account Id",
      responsive: ["sm"],
      width: 70,
      dataIndex: "accountId",
      key: "accountId",
      sorter: (a: any, b: any) => a.accountId.length - b.accountId.length,
    },
    {
      title: "TXN No",
      responsive: ["sm"],
      width: 70,
      dataIndex: "transNo",
      key: "transNo",
      sorter: (a: any, b: any) => a.transNo.length - b.transNo.length,
    },
    {
      title: <p className="text-center ">Account Name</p>,
      responsive: ["sm"],
      width: 200,
      dataIndex: "accountName",
      key: "accountName ",
      sorter: (a: any, b: any) => a.accountName.length - b.accountName.length,
      render: (text) => <p className="text-center ">{text}</p>,
    },

    {
      title: "Parent Account",
      responsive: ["sm"],
      width: 150,
      dataIndex: "parent",
      key: "parent",
      sorter: (a: any, b: any) => a.parent.length - b.parent.length,
      render: (text) => (
        <div className="flex flex-col capitalize">
          <p className="" onClick={() => text}>
            {text?.accountName || "no parent Account"}
          </p>
          <p className="text-[12px]" onClick={() => text}>
            {text?.accountId || ""}
          </p>
        </div>
      ),
    },
    {
      title: "Account Type",
      responsive: ["sm"],
      width: 90,
      dataIndex: "accountType",
      key: "accountType ",
    },
    {
      title: "Account Level ",
      responsive: ["sm"],
      width: 100,
      dataIndex: "accountLevel",
      key: "accountLevel ",
    },
  ];

  const fetch = async () => {
    setIsloading(true);

    const fetchTableData = await depositStore.getAllChartOfAccount();
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

  useEffect(() => {
    fetch();
  }, []);

  return (
    <Spin tip="fetching...." spinning={isloading} className="text-[red] ">
      <main className=" min-h-screen flex flex-col gap-1 w-[95%]  mx-auto ">
        <div className="flex justify-between items-center mb-3">
          <Input
            className="w-[200px]"
            placeholder="Search by Name, TXN ID."
            prefix={<SearchOutlined />}
          />
          <Link href="/user/accounting/maintain/chart-of-accounts/New-Chart-Of-Account">
            <button
              type="button"
              className="inline-flex text-white capitalize bg-[#194BFB] border-0 py-2 px-6 focus:outline-none hover:bg-[#191dfb] rounded text-md"
            >
              Add New
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
              // onSelect={setbatchPostStatus}
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
