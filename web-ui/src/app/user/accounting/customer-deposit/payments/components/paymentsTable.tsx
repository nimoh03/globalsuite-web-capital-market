import { Space, Table, Tag } from "antd";
import { useState } from "react";

const columns = [
  {
    title: "Account",
    dataIndex: "account",
    key: "account",
    sorter: (a: any, b: any) => a.account.length - b.account.length,
  },
  {
    title: "Txn#",
    dataIndex: "txn",
    key: "txn",
    sorter: (a: any, b: any) => a.txn.length - b.txn.length,
  },
  {
    title: "Amount",
    dataIndex: "amount",
    key: "amount",
    sorter: (a: any, b: any) => a.amount - b.amount,
  },
  {
    title: "Status",
    dataIndex: "status",
    key: "status",
    render: (value: string) => {
      let textColor = "text-orange-100";
      let bgColor = "bg-alert-error-light";

      if (value === "approved") {
        textColor = "text-alert-success";
        bgColor = "bg-green-100";
      } else if (value === "reversed") {
        textColor = "text-alert-warning";
        bgColor = "bg-yellow-100";
      }

      return (
        <Tag className={`${textColor} ${bgColor} border-none`}>
          {value.toUpperCase()}
        </Tag>
      );
    },
  },
  {
    title: "Action",
    key: "action",
    render: (_: any, record: any) => (
      <Space size="middle">
        <a>View</a>
        <a>Delete</a>
      </Space>
    ),
  },
];

const mockData: any = [];

for (let i = 0; i < 100; i++) {
  mockData.push({
    key: i,
    account: `Edward King ${i}`,
    txn: `TXN ${i}`,
    amount: 32,
    status: i % 3 === 0 ? "approved" : i % 2 === 0 ? "reversed" : "pending",
  });
}

export const PaymentTable = () => {
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [loading, setLoading] = useState(false);

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

  return (
    <Table
      rowSelection={{
        type: "checkbox",
        ...rowSelection,
      }}
      columns={columns}
      dataSource={mockData}
      sticky={{
        offsetHeader: 50,
      }}
    />
  );
};
