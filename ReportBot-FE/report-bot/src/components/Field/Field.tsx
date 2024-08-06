import { Box } from "@mui/material";

const Field = (props: { label: string, content: string }) => {
  return (
    props.content !== '' ? (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          width: '100%',
        }}
      >
        <Box sx={{
          fontSize: '12px',
          color: '#666666',
          whiteSpace: 'nowrap',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        }}>{props.label}</Box>
        <Box sx={{
          fontSize: '15px',
          fontWeight: 'bold',
          whiteSpace: 'nowrap',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        }}>{props.content}</Box>
        <Box sx={{ borderBottom: '1px solid #666666', width: '100%' }} />
      </Box>
    ) : null
  );
}

export default Field;